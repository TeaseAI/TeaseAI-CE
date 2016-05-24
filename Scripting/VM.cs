using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using MyResources;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// Main 'world' object, needs to know about all scripts, personalities, controllers, functions, etc..
	/// </summary>
	public class VM : IKeyed
	{
		public delegate Variable Function(Context sender, Variable[] args);

		private Thread thread = null;
		private volatile bool threadRun = false;
		public bool IsRunning { get { return threadRun; } }

		private Random random = new Random();

		private ConcurrentDictionary<string, Function> functions = new ConcurrentDictionary<string, Function>();

		private ReaderWriterLockSlim personControlLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		//private Dictionary<string, Personality> personalities = new Dictionary<string, Personality>();
		private List<Controller> controllers = new List<Controller>();
		private List<Dictionary<string, string>> inputReplace;

		private ReaderWriterLockSlim scriptsLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		private List<GroupInfo> scriptGroups = new List<GroupInfo>();
		private List<Script> scriptSetups = new List<Script>();
		private Dictionary<string, List<BlockBase>> scriptResponses = new Dictionary<string, List<BlockBase>>();

		private KeyedDictionary<Variable<Personality>> personalities = new KeyedDictionary<Variable<Personality>>(false);

		// ToDo : Rename:
		private BlockGroup<Script> allscripts;

		private volatile int _dirty = 0;
		public bool Dirty { get { return _dirty != 0; } internal set { Interlocked.Exchange(ref _dirty, value ? 1 : 0); } }

		public const char IndentChar = '\t';
		public static readonly char[] Punctuation = new char[] { '\'', '!', '?', '.', ',' };
		private static readonly char[] keySeparator = { '.' };

		public VM()
		{
			allscripts = new BlockGroup<Script>(this);
		}

		#region Tick thread
		/// <summary> Starts the controller update thread </summary>
		public void Start()
		{
			if (Dirty)
			{
				Trace.WriteLine(StringsScripting.Log_VM_Dirty);
			}
			else if (thread == null || !thread.IsAlive)
			{
				threadRun = true;
				thread = new Thread(threadTick);
				thread.Start();
			}
		}
		/// <summary> Stop controller updating. </summary>
		public void Stop()
		{
			threadRun = false;
			if (thread != null)
			{
				foreach (var c in controllers) // stop all timmers
					c.timmer.Stop();
				for (int i = 0; i < 10; ++i) // wait one second for thread to stop on it's own.
					if (thread.IsAlive)
						Thread.Sleep(100);
				if (thread.IsAlive)
				{
					thread.Abort();
					thread.Join();
				}
				thread = null;
			}
		}

		/// <summary> Updates all controllers. </summary>
		private void threadTick()
		{
			try
			{
				while (threadRun && !Dirty)
				{
					// update all controllers
					personControlLock.EnterReadLock();
					try
					{
						foreach (var c in controllers)
						{
							if (!c.timmer.IsRunning)
								c.timmer.Start();
							if (c.timmer.ElapsedMilliseconds > c.Interval)
							{
								c.timmer.Stop();
								c.timmer.Reset();
								c.timmer.Start();
								c.Tick();
							}
						}
					}
					finally
					{ personControlLock.ExitReadLock(); }
					Thread.Sleep(50);
				}
			}
			catch (ThreadAbortException)
			{
				return;
			}
		}
		#endregion

		#region Personality stuff

		/// <summary>
		/// Craete a new personality with given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns>New Personality or null if name already exits.</returns>
		public Personality CreatePersonality(string name)
		{
			var id = KeyClean(name);
			Dirty = true;
			// If personality with id already exists, add number.
			if (personalities.ContainsKey(id))
			{
				int num = 1;
				while (personalities.ContainsKey(id + (++num)))
				{ }
				id = id + num;
			}

			var p = new Personality(this, name, id);
			personalities[id] = new Variable<Personality>(p);
			return p;
		}

		public bool TryGetPersonality(string name, out Personality p)
		{
			Variable<Personality> vP = null;
			if (personalities.TryGetValue(KeyClean(name), out vP) && vP.IsSet)
			{
				p = vP.Value;
				return true;
			}
			p = null;
			return false;
		}

		public bool ChangePersonalityID(Personality p, string newID)
		{
			newID = KeyClean(newID);
			if (personalities.ContainsKey(newID))
				return false;
			Dirty = true;
			Variable<Personality> var;
			if (personalities.TryRemove(p.ID, out var))
				personalities[newID] = var;
			else
				personalities[newID] = new Variable<Personality>(p);
			p.ID = newID;
			return true;
		}

		/// <summary>
		/// Creates a new controller for the given personality.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public Controller CreateController(Personality p, string id)
		{
			personControlLock.EnterWriteLock();
			try
			{
				var c = new Controller(p, id);
				controllers.Add(c);
				return c;
			}
			finally
			{ personControlLock.ExitWriteLock(); }
		}

		public Controller[] GetControllers()
		{
			personControlLock.EnterWriteLock();
			try
			{ return controllers.ToArray(); }
			finally
			{ personControlLock.ExitWriteLock(); }
		}

		public Personality[] GetPersonalities()
		{
			var p = personalities.Values.ToArray();
			var result = new Personality[p.Length];
			for (int i = 0; i < result.Length; ++i)
				result[i] = p[i].Value;
			return result;
		}

		#endregion

		public Script QueryScript(Variable query, Logger log)
		{
			if (query == null || !query.IsSet)
				log.Error(StringsScripting.Query_empty);
			else if (query.Value is VType.Query)
				return QueryScript((VType.Query)query.Value, log);
			else if (query.Value is string)
				return QueryScript((string)query.Value, log);
			else
				log.ErrorF(StringsScripting.Formatted_Invalid_Type, "Query", query.Value.GetType().Name);
			return null;
		}
		public Script QueryScript(VType.Query query, Logger log)
		{
			if (query == null)
			{
				log.Error(StringsScripting.Query_empty);
				return null;
			}

			var list = allscripts.GetAll().ToList();
			VType.Query.QueryReduceByTag(list, query, log);
			if (list == null || list.Count == 0)
			{
				log.Error(StringsScripting.Query_empty);
				return null;
			}
			// ToDo6: Make not random.
			int r = random.Next(0, list.Count);
			return list[r].Value;
		}

		public Variable Get(Key key, Logger log = null)
		{
			if (key.AtEnd)
			{
				Logger.LogF(log, Logger.Level.Error, StringsScripting.Formatted_IKeyed_Cannot_return_self, key, GetType());
				return null;
			}
			switch (key.Peek)
			{
				case "personality":
					return personalities.Get(++key, log);
				case "script":
					return allscripts.Get(key, log);
				case "list":
					return allscripts.Get(key, log);
				default:
					{
						Function func;
						if (functions.TryGetValue(key.Peek, out func))
							return new Variable(func);
						Logger.LogF(log, Logger.Level.Error, StringsScripting.Formatted_Function_not_found, key);
						return null;
					}
			}
		}

		public GroupInfo[] GetGroups()
		{
			scriptsLock.EnterReadLock();
			try
			{
				return scriptGroups.ToArray();
			}
			finally
			{ scriptsLock.ExitReadLock(); }
		}

		public void AddFunction(string name, Function func)
		{
			Dirty = true;
			functions[KeyClean(name)] = func;
		}
		public void AddFunction(Function func)
		{
			AddFunction(func.Method.Name, func);
		}

		/// <summary> Runs all setup scripts on the personality. </summary>
		/// <param name="p"></param>
		internal void RunSetupOn(Personality p)
		{
			var c = new Controller(p, "DUMMY");
			var sb = new StringBuilder();
			scriptsLock.EnterReadLock();
			try
			{
				foreach (var s in scriptSetups)
					runThroughScript(p, c, s, sb);
			}
			finally
			{ scriptsLock.ExitReadLock(); }
		}
		/// <summary> Add script to controller, call next until false. </summary>
		/// <returns> false if valid is not passed </returns>
		private bool runThroughScript(Personality p, Controller c, BlockBase s, StringBuilder sb)
		{
			if (s.Valid != BlockBase.Validation.Passed)
				return false;
			c.Add(s);
			while (c.next(sb))
			{
			}
			return true;
		}

		private void addScript(string type, string rootKey, string key, Script script, string[] tags, string[] responses)
		{
			allscripts.TryAdd(type, rootKey, key, script);

			// responses
			if (responses != null)
			{
				foreach (string keyword in responses)
				{
					if (keyword == null || keyword.Length == 0)
						continue;
					List<BlockBase> list;
					if (!scriptResponses.TryGetValue(keyword, out list))
						list = scriptResponses[keyword] = new List<BlockBase>();
					list.Add(script);
				}
			}
		}

		#region Loading files

		/// <summary>
		/// Load files in path and all sub directories.
		/// </summary>
		public void LoadFromDirectory(string path)
		{
			var log = new Logger("Loader");
			if (!Directory.Exists(path))
			{
				log.Error(string.Format(StringsScripting.Formatted_Directory_not_found, path));
				return;
			}

			using (new LogTimed(log, string.Format(StringsScripting.Formatted_Log_Load_Start, path), string.Format(StringsScripting.Formatted_Log_Load_Finish, path)))
			{
				// load all input replace files.
				var files = Directory.GetFiles(path, "input replace.csv", SearchOption.AllDirectories);
				personControlLock.EnterWriteLock();
				try
				{
					if (inputReplace == null)
						inputReplace = new List<Dictionary<string, string>>();
					foreach (var file in files)
					{
						log.Info(string.Format(StringsScripting.Formatted_Log_Loading_file, file));
						Logger fileLog;
						var rawLines = getFileLines(file, out fileLog);
						if (rawLines == null)
							continue;
						parseInputReplace(rawLines, fileLog);
					}
				}
				finally
				{ personControlLock.ExitWriteLock(); }

				// load all .vtscript files.
				files = Directory.GetFiles(path, "*.vtscript", SearchOption.AllDirectories);
				foreach (string file in files)
				{
					log.Info(string.Format(StringsScripting.Formatted_Log_Loading_file, file));
					Logger fileLog;
					// get all lines from the file.
					var rawLines = getFileLines(file, out fileLog);
					if (rawLines == null)
						continue;
					// get the base script key from the file name.
					string fileKey = KeyClean(Path.GetFileNameWithoutExtension(file));
					// parse as a group
					parseScriptGroup(rawLines, file, fileKey, fileLog);
				}
			}
		}

		/// <summary> Gets all lines in file, returns null on IOException. </summary>
		private List<string> getFileLines(string file, out Logger log)
		{
			log = new Logger(file);
			var result = new List<string>();
			try
			{
				using (var stream = new StreamReader(file))
					while (!stream.EndOfStream)
						result.Add(stream.ReadLine());
			}
			catch (IOException ex)
			{
				log.Error(ex.Message);
				return null;
			}
			return result;
		}

		#endregion

		#region Parse scripts

		/// <summary>
		/// Parses raw lines in to the system.
		/// </summary>
		private void parseScriptGroup(List<string> rawLines, string filePath, string fileKey, Logger fileLog)
		{
			// split-up the lines in to indatation based blocks.
			var group = new GroupInfo(filePath, fileKey, fileLog);
			scriptsLock.EnterWriteLock();
			scriptGroups.Add(group);
			scriptsLock.ExitWriteLock();
			var blocks = group.Blocks;
			int currentLine = 0;
			string blockKey = null;
			string[] blockTags = null;
			string[] blockResponses = null;
			int blockLine = 0;
			while (currentLine < rawLines.Count)
			{
				string str = rawLines[currentLine];
				int indent = 0;
				fileLog.SetId(currentLine);
				if (parseCutLine(ref str, ref indent)) // line empty?
				{
					if (indent == 0) // indent 0 defines what the key of the upcoming object is.
					{
						List<string[]> args;
						parseBlockStart(str, out blockKey, out args, fileLog);
						blockTags = null;
						blockResponses = null;
						if (args.Count > 0)
							blockTags = KeyClean(args[0], fileLog);
						if (args.Count > 1)
							blockResponses = SanitizeInputReplace(args[1]);

						blockLine = currentLine;
						// make sure key is a valid type.
						var rootKey = KeySplit(blockKey)[0];
						if (rootKey != "script" && rootKey != "list" && rootKey != "setup" && rootKey != "personality")
						{
							fileLog.Error(string.Format(StringsScripting.Formatted_Error_Invalid_root_type, rootKey), currentLine);
							break;
						}
						++currentLine;
					}
					else if (indent > 0)
					{
						if (blockKey == null)
						{
							fileLog.Error(StringsScripting.Invalid_Indentation, currentLine);
							break;
						}
						var log = new Logger(fileKey + "." + blockKey);
						var lines = parseBlock(rawLines, ref currentLine, indent, log);
						if (lines == null)
							blocks.Add(new BlockBase(blockKey, null, blockTags, group, log));
						else
						{
							// Figureout type of script, then add it.
							var keySplit = KeySplit(blockKey);
							if (keySplit.Length == 1)
							{
								if (keySplit[0] == "setup")
								{
									scriptsLock.EnterWriteLock();
									try
									{ scriptSetups.Add(new Script(this, false, blockKey, lines, blockTags, group, log)); }
									finally
									{ scriptsLock.ExitWriteLock(); }
									// warn if has tags or responses
									if (blockTags != null && blockTags.Length > 0)
										fileLog.Warning(StringsScripting.Setup_has_tags, blockLine);
									if (blockResponses != null && blockResponses.Length >= 0)
										fileLog.Warning(StringsScripting.Setup_has_responses, blockLine);
								}
								else
								{
									fileLog.Error(string.Format(StringsScripting.Formatted_Error_Invalid_root_type, keySplit[0]), blockLine);
									break;
								}
							}
							else if (keySplit.Length == 2)
							{
								if (keySplit[1] == null || keySplit[1].Length == 0)
								{
									fileLog.Warning(StringsScripting.Warning_Empty_sub_key);
									continue;
								}
								var key = fileKey + '.' + keySplit[1];
								switch (keySplit[0])
								{
									case "script":
									case "list":
										{
											var script = new Script(this, keySplit[0] == "list", blockKey, lines, blockTags, group, log);
											blocks.Add(script);
											addScript(keySplit[0], fileKey, keySplit[1], script, blockTags, blockResponses);
										}
										break;
									case "personality":
										{
											key = keySplit[1];
											// does personality already exist?
											Personality p = null;
											Variable<Personality> vP = null;
											if (personalities.TryGetValue(key, out vP))
												p = vP.Value;
											// if not create new.
											else
												p = CreatePersonality(key);
											// run through the script to fill the personalities variables.
											var script = new Script(this, false, blockKey, lines, null, group, log);
											var c = new Controller(p, "DUMMY");
											var sb = new StringBuilder();
											validateScript(c, script, null, sb);
											runThroughScript(p, c, script, sb);
										}
										break;
									default:
										fileLog.Error(string.Format(StringsScripting.Formatted_Error_Invalid_root_type, keySplit[0]), blockLine);
										return;
								}
							}
						}
					}
				}
				else
					++currentLine;
			}
			return;
		}

		/// <summary>
		/// Parses rawLines in to blocks of code recursively based on blockIndent.
		/// </summary>
		/// <param name="rawLines"></param>
		/// <param name="currentLine"></param>
		/// <param name="blockIndent">Indent level this block is at.</param>
		/// <returns>Block with lines, or null if zero lines.</returns>
		private static Line[] parseBlock(List<string> rawLines, ref int currentLine, int blockIndent, Logger log)
		{
			// temp list of lines, until we are finished parsing.
			var lines = new List<Line>();

			string lineData;
			int lineIndent = 0;
			int indentDifference;
			// Note: This loop is picky, do NOT edit unilss you understand what is happening.
			// currentLine should be added to once we are done with the line.
			while (currentLine < rawLines.Count)
			{
				log.SetId(currentLine + 1);
				// get raw line, cut it and get indent level.
				lineData = rawLines[currentLine];
				if (parseCutLine(ref lineData, ref lineIndent))
				{
					indentDifference = lineIndent - blockIndent;
					// if indentation difference is negative, then exit the block.
					if (indentDifference < 0)
					{
						break;
					}
					// indentation unchanged, so just add the line.
					else if (indentDifference == 0)
					{
						lines.Add(new Line(currentLine + 1, lineData, null));
						++currentLine;
					}
					// next level of indentation. Parse as a sub block, then add to last line.
					else if (indentDifference == +1)
					{
						Line[] block = parseBlock(rawLines, ref currentLine, lineIndent, log);
						if (block == null) // ignore block if empty
							continue;
						if (lines.Count == 0)
						{
							log.Warning(StringsScripting.Invalid_Indentation);
							lines.Add(new Line(-1, "", block));
						}
						else
						{
							// replace the last line, with the new lines.
							var tmp = lines[lines.Count - 1];
							lines[lines.Count - 1] = new Line(tmp.LineNumber, tmp.Data, block);
						}
					}
					else // invalid indentation.
					{
						log.Warning(StringsScripting.Invalid_Indentation);
						++currentLine;
					}
				}
				else // line was empty
					++currentLine;
			}

			if (lines.Count == 0)
				return null;
			return lines.ToArray();
		}

		/// <summary>
		/// Counts indentation, and trims white-space and comments from str.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="indentCount"></param>
		/// <returns>false if str would be empty.</returns>
		private static bool parseCutLine(ref string str, ref int indentCount)
		{
			if (str.Length == 0)
				return false;
			// Count indent level, and find the start of the text.
			int start = 0;
			while (true)
			{
				if (start == str.Length)
					return false;
				if (str[start] != IndentChar)
					break;
				++start;
			}
			indentCount = start; // sense we are using a single character as the indent, then the start is equal to the indent.

			// Find the end of the text, ignoring comments. Note: we are also trimming off any white-space at the end.
			int end = 0;
			char c;
			for (int i = start; i < str.Length; ++i)
			{
				c = str[i];
				if (!char.IsWhiteSpace(c))
				{
					// Comment?
					if (c == '/' &&
						(i == 0 || str[i - 1] != '\\') && // make sure the last character is not the escape character.
						(i + 1 < str.Length && str[i + 1] == '/')) // and the next character is /
					{
						break;
					}
					else // not a comment, so set the end sense this is not white-space.
						end = i;
				}
			}
			if (end <= start)
				return false;
			// apply the start/end
			str = str.Substring(start, end - start + 1);
			return true;
		}

		private static void parseBlockStart(string str, out string key, out List<string[]> args, Logger log)
		{
			args = new List<string[]>();
			var sb = new StringBuilder();
			bool iskey = true;
			int i = 0;
			char c;
			while (i < str.Length)
			{
				c = str[i++];
				if (c == '[')
				{
					args.Add(parseStringArray(str, ref i, log));
					iskey = false;
				}
				else if (iskey && c != ' ')
					sb.Append(c);
			}
			key = KeyClean(sb.ToString(), log);
		}

		/// <summary> Parse "" as a string</summary>
		/// <param name="i">should be the next character after the first " </param>
		private static string parseStringQuoted(string str, ref int i, Logger log)
		{
			var sb = new StringBuilder();
			char c;
			while (i < str.Length)
			{
				c = str[i++];

				if (c == '"') // string end
					return sb.ToString();

				// escape
				else if (c == '\\' && i + 1 < str.Length)
				{
					char next = str[i++];
					if (next == '\\')
						sb.Append('\\');
					else if (next == 'n')
						sb.Append('\n');
					else if (next == '"')
						sb.Append('"');
					else
						log.Warning(string.Format(StringsScripting.Formatted_Invalid_string_escape_character, next), -1, i);
				}
				else
					sb.Append(c);
			}
			return sb.ToString();
		}

		/// <summary> Parse [] as an array of strings</summary>
		/// <param name="i"> should be the next character after [ </param>
		private static string[] parseStringArray(string str, ref int i, Logger log)
		{
			var result = new List<string>();
			var sb = new StringBuilder();
			char c;
			int lastLength = 0; // ignore white-space on the end. (TrimEnd)
			while (i < str.Length)
			{
				c = str[i++];
				switch (c)
				{
					case '"':
						if (sb.Length == 0)
							sb.Append(parseStringQuoted(str, ref i, log));
						else
							sb.Append(c);
						lastLength = sb.Length;
						break;
					case '\t':
					case ' ':
						// ignore what-space on the biginning. (TrimStart)
						if (sb.Length > 0)
							sb.Append(c);
						break;
					case ',':
						result.Add(sb.ToString(0, lastLength));
						sb.Clear();
						break;
					case ']': // End
						result.Add(sb.ToString(0, lastLength));
						if (result.Count == 0)
							return null;
						return result.ToArray();
					default:
						sb.Append(c);
						lastLength = sb.Length;
						break;
				}
			}
			result.Add(sb.ToString(0, lastLength));
			if (result.Count == 0)
				return null;
			return result.ToArray();
		}

		#endregion

		#region Validation

		public void Validate()
		{
			var log = new Logger("Validator");
			using (new LogTimed(log, "Starting", "Finished"))
			{
				// dummy variables used to run scripts without effecting user data.
				var p = new Personality(this, "tmpValidator", "tmpValidator");
				var c = new Controller(p, "DUMMY");
				var output = new StringBuilder();
				var vars = new Dictionary<string, Variable>();


				personControlLock.EnterReadLock();
				try
				{
					if (personalities.Count == 0)
						log.Error(StringsScripting.No_personalities);
					if (inputReplace == null || inputReplace.Count == 0)
						log.Warning(StringsScripting.No_input_replace);
				}
				finally
				{ personControlLock.ExitReadLock(); }



				if (allscripts.IsEmpty)
					log.Error(StringsScripting.No_scripts);


				// validate startup scripts.
				foreach (var s in scriptSetups)
					validateScript(c, s, vars, output);

				// validate all other scripts.
				foreach (var s in allscripts.GetAll())
					if (s.IsSet)
						validateScript(c, s, vars, output);
			}
			// ToDo 2: Scripts can change inbetwen here and validation, so we could be dirty but dirty being false.
			Dirty = false;
		}
		private void validateScript(Controller c, Script s, Dictionary<string, Variable> vars, StringBuilder output)
		{
			// if script has never been validated, but has errors, then do not validate, they are parse errors.
			if (s.Valid == BlockBase.Validation.NeverRan && s.Log.ErrorCount > 0)
			{
				s.SetValid(false); // will set valid to failed.
				return;
			}

			s.SetValid(true);
			if (s.List)
			{
				vars.Clear();
				s.ExecuteAsList(new Context(c, s, s, 0, vars), output);
			}
			else
			{
				c.Add(s);
				while (c.next(output))
				{

				}
			}
			s.SetValid(false);
		}
		#endregion

		#region Executing lines
		/// <summary>
		/// Execute a line of code.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="line"></param>
		/// <param name="output"></param>
		internal void ExecLine(Context sender, string line, StringBuilder output)
		{
			var log = sender.Root.Log;
			string key;
			Variable[] args;

			int i = 0;
			char c;
			while (i < line.Length && !sender.ExitLine)
			{
				c = line[i];
				++i;
				switch (c)
				{
					case '#':
					case '@':
						execSplitCommand(sender, line, ref i, out key, out args);
						if (key != null)
						{
							Variable variable = sender.Get(new Key(key, sender), log);
							if (variable == null || variable.IsSet == false)
								continue;
							var func = variable.Value as Function;
							if (func != null)
								variable = func(sender, args);
							// ToDo : Do we need to do anything to other value types?

							// output if @
							if (c == '@' && variable != null)
								variable.WriteValueUser(sender, output);
						}
						else
						{
							if (c == '@' && args != null && args.Length > 0 && args[0] != null)
								args[0].WriteValueUser(sender, output);
						}
						break;

					//case '\\':
					// ToDo : escape character.
					//break;
					default:
						output.Append(c);
						break;
				}
			}
		}
		/// <summary> gets key and parentheses as args. </summary>
		/// <param name="i"> indexer, expected to start on the first character of the key. </param>
		/// <param name="key"> set to null if no key. </param>
		/// <param name="args"> never null </param>
		internal void execSplitCommand(Context sender, string str, ref int i, out string key, out Variable[] args)
		{
			args = null;
			var sb = new StringBuilder();
			char c;
			while (i < str.Length)
			{
				c = str[i];
				if (c == ' ')
					break;
				else if (c == '(')
				{
					++i;
					args = execParentheses(sender, str, ref i);
					break;
				}
				else
					++i;
				sb.Append(c);
			}
			if (sb.Length > 0)
			{
				bool isFloat;
				bool isPercent;
				key = KeyClean(sb.ToString(), out isFloat, out isPercent, sender.Root.Log);
				if (isFloat || isPercent)
					sender.Root.Log.Warning(string.Format(StringsScripting.Formatted_Expected_key_got_number, key));
			}
			else
				key = null;
			if (args == null)
				args = new Variable[0];
		}

		/// <summary> temporary object to hold values and operators while executing parentheses. </summary> 
		private struct execParenthItem
		{
			public Operators Operator;
			public Variable Value;
			public bool IsValue { get { return Value != null; } }
			public override string ToString()
			{
				if (IsValue)
					return "value: " + Value.ToString();
				return "op: " + Operator.ToString();
			}
			public static implicit operator execParenthItem(Variable value)
			{
				if (value == null)
					return new execParenthItem() { Value = new Variable() };
				return new execParenthItem() { Value = value };
			}
			public static implicit operator execParenthItem(Operators op)
			{ return new execParenthItem() { Operator = op }; }
		}
		/// <summary>
		/// Recursively parses and executes everything in the parentheses, then returns variable array.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="str"></param>
		/// <param name="i"> indexer, must be equal to the char after '(' </param>
		/// <returns></returns>
		private Variable[] execParentheses(Context sender, string str, ref int i)
		{
			var log = sender.Root.Log;
			int start = i;
			var outArgs = new List<Variable>();
			outArgs.Add(null); // Set to the output before return.
			var items = new List<execParenthItem>();

			// Split in to lists of values and operators.
			var sb = new StringBuilder();
			bool finished = false;
			char c;
			while (i < str.Length && !finished)
			{
				c = str[i];
				switch (c)
				{
					// string
					case '"':
						execParenthCheckAdd(sender, items, sb);
						++i;
						items.Add(new Variable(parseStringQuoted(str, ref i, log)));
						continue;

					// finish parentheses
					case ')':
						finished = true;
						break;
					// sub parentheses
					case '(':
						{
							int count = items.Count;
							execParenthCheckAdd(sender, items, sb);
							// check if it's attached to a function
							bool funcParenth = false;
							if (count < items.Count && items[items.Count - 1].IsValue)
							{
								var v = items[items.Count - 1].Value;
								funcParenth = v.IsSet && v.Value is Function;
							}

							++i;
							var _char = i;
							var args = execParentheses(sender, str, ref i);

							if (funcParenth) // simply add the whole array as-is.
								items.Add(new Variable(args));
							else if (args.Length == 0)
								log.Warning(StringsScripting.Sub_parentheses_zero_arguments, -1, _char);
							else
								items.Add(args[0]); // sense it's not for a function, we only care about the first arg.

							if (args.Length > 1)
								log.Warning(StringsScripting.Sub_parentheses_too_many_arguments, -1, _char);
						}
						continue;

					// new arg
					case ',':
						execParenthCheckAdd(sender, items, sb);
						++i;
						outArgs.AddRange(execParentheses(sender, str, ref i));
						finished = true; // finished, sense execParentheses will go until end.
						continue;

					// white-space seprates stuffs
					case ' ':
					case '\t':
						execParenthCheckAdd(sender, items, sb);
						break;

					// equal and assign
					case '=':
						execParenthCheckAdd(sender, items, sb);
						// (a=b) is not eqqual to (a==b). The first assigns b to a and returns a, second returns bool if a equals b.
						// so we must use different operators.
						if (i + 1 < str.Length && str[i + 1] == '=') // ==
						{ ++i; items.Add(Operators.Equal); }
						else // just a single =
							items.Add(Operators.Assign);
						break;

					// math
					case '*':
						execParenthCheckAdd(sender, items, sb);
						items.Add(Operators.Multiply);
						break;
					case '/':
						execParenthCheckAdd(sender, items, sb);
						items.Add(Operators.Divide);
						break;
					case '+':
						execParenthCheckAdd(sender, items, sb);
						items.Add(Operators.Add);
						break;
					case '-':
						execParenthCheckAdd(sender, items, sb);
						items.Add(Operators.Subtract);
						break;

					// logic
					// "and" "or" are handled in execParenthCheckAdd 
					case '>':
						execParenthCheckAdd(sender, items, sb);
						items.Add(Operators.More);
						break;
					case '<':
						execParenthCheckAdd(sender, items, sb);
						items.Add(Operators.Less);
						break;

					default:
						sb.Append(c);
						break;
				}
				++i;
			}
			execParenthCheckAdd(sender, items, sb);
			log.SetId(-1, start);

			// check if empty
			if (items.Count == 0)
			{
				log.Warning(StringsScripting.Parentheses_empty);
				return new Variable[0];
			}
			else if (items[0].IsValue == false && items[0].Operator != Operators.Not)
			{
				log.Error(StringsScripting.Parentheses_first_item_is_not_variable);
				return new Variable[0];
			}

			// Evaluate
			int j;

			// Operator precedence
			// 1. Parentheses          ( )
			// 2. Execute functions
			// 3. Logic Not            not
			// 4. Multiply & Divide    * /
			// 5. Add & Subtract       + -
			// 6. logic 1              > < ==
			// 7. logic 2             and or
			// 8. Assignment          =
			//Note: 'not' and assignment goes right to left.

			// 1. Done when we parse
			// 2. Execute functions
			j = 0;
			for (; j < items.Count; ++j)
			{
				if (items[j].IsValue)
				{
					var func = items[j].Value.Value as Function;
					if (func == null)
						continue;
					Variable[] args = null;
					// check if args is next item
					if (j + 1 < items.Count && items[j + 1].IsValue)
					{
						var argvar = items[j + 1].Value.Value as Variable[];
						if (argvar == null)
						{
							log.Error(StringsScripting.Parentheses_Expected_function_arguments);
							return new Variable[0];
						}
						args = argvar;
						items.RemoveAt(j + 1);
					}
					if (args == null)
						args = new Variable[0];
					items[j] = func(sender, args);
				}
			}

			// 3. Logic not
			j = items.Count - 2;
			while (j >= 0)
			{
				if (!items[j].IsValue && items[j].Operator == Operators.Not)
				{
					var r = items[j + 1].Value;
					items[j] = Variable.Evaluate(sender, null, Operators.Not, r);
					items.RemoveAt(j + 1);
				}
				--j;
			}

			// At this point it is required that there is exactly one operator inbetween each variable.
			j = 0;
			while (j + 2 < items.Count)
			{
				var l = items[j];
				var o = items[j + 1];
				var r = items[j + 2];
				if (!l.IsValue || !r.IsValue)
				{
					if (!l.IsValue)
						log.ErrorF(StringsScripting.Formatted_Expected_variable_got_operator, l.Operator);
					if (!r.IsValue)
						log.ErrorF(StringsScripting.Formatted_Expected_variable_got_operator, r.Operator);
					return new Variable[0];
				}
				else if (o.IsValue)
				{
					log.Error(StringsScripting.Expected_operator_got_variable);
					return new Variable[0];
				}
				else if (o.Operator == Operators.Not)
				{
					log.ErrorF(StringsScripting.Formatted_Unexpected_Operator, o.ToString());
					return new Variable[0];
				}
				j += 2;
			}

			// 4. Multiply & Divide
			j = 0;
			while (j + 2 < items.Count)
			{
				var l = items[j].Value;
				var op = items[j + 1].Operator;
				var r = items[j + 2].Value;
				if (op == Operators.Multiply || op == Operators.Divide)
				{
					items[j] = Variable.Evaluate(sender, l, op, r);
					items.RemoveRange(j + 1, 2);
				}
				else
					j += 2;
			}
			// 5. Add & Subtract
			j = 0;
			while (j + 2 < items.Count)
			{
				var l = items[j].Value;
				var op = items[j + 1].Operator;
				var r = items[j + 2].Value;
				if (op == Operators.Add || op == Operators.Subtract)
				{
					items[j] = Variable.Evaluate(sender, l, op, r);
					items.RemoveRange(j + 1, 2);
				}
				else
					j += 2;
			}
			// 6. logic 1
			j = 0;
			while (j + 2 < items.Count)
			{
				var l = items[j].Value;
				var op = items[j + 1].Operator;
				var r = items[j + 2].Value;
				if (op == Operators.More || op == Operators.Less || op == Operators.Equal)
				{
					items[j] = Variable.Evaluate(sender, l, op, r);
					items.RemoveRange(j + 1, 2);
				}
				else
					j += 2;
			}
			// 7. logic 2
			j = 0;
			while (j + 2 < items.Count)
			{
				var l = items[j].Value;
				var op = items[j + 1].Operator;
				var r = items[j + 2].Value;
				if (op == Operators.And || op == Operators.Or)
				{
					items[j] = Variable.Evaluate(sender, l, op, r);
					items.RemoveRange(j + 1, 2);
				}
				else
					j += 2;
			}
			// 8. Assignment
			j = items.Count - 1;
			while (j - 2 >= 0)
			{
				var l = items[j - 2].Value;
				var op = items[j - 1].Operator;
				var r = items[j - 0].Value;
				if (op == Operators.Assign)
				{
					items[j - 2] = Variable.Evaluate(sender, l, op, r);
					items.RemoveRange(j - 1, 2);
				}
				j -= 2;
			}

			// finily finished
			if (items.Count != 1)
			{
				// ToDo : probley just return a array of the items? (if not add to StringsScripting.txt)
				log.Error(string.Format("execParentheses items.Count is {0}, expecting a count of 1!", items.Count));
				return new Variable[0];
			}
			outArgs[0] = items[0].Value;
			return outArgs.ToArray();
		}
		/// <summary>
		/// Check whats in sb, then add it to the list.
		/// [float, bool, and, or, variable]
		/// </summary>
		/// <returns>false if nothing was added.</returns>
		private void execParenthCheckAdd(Context sender, List<execParenthItem> items, StringBuilder sb)
		{
			if (sb.Length == 0)
				return;

			bool isFloat;
			bool isPercent;
			string str = KeyClean(sb.ToString(), out isFloat, out isPercent, sender.Root.Log);
			sb.Clear();
			if (str == null)
				return;
			// is str float?
			if (isFloat)
				items.Add(new Variable(float.Parse(str)));
			// percent to float?
			else if (isPercent)
				items.Add(new Variable(float.Parse(str) * 0.01f));
			// bool
			else if (str == "true")
				items.Add(new Variable(true));
			else if (str == "false")
				items.Add(new Variable(false));
			// logic operators
			else if (str == "not")
				items.Add(Operators.Not);
			else if (str == "and")
				items.Add(Operators.And);
			else if (str == "or")
				items.Add(Operators.Or);
			else // variable
				items.Add(sender.Get(new Key(str, sender)));
			return;
		}


		#endregion

		#region Input to shorthand

		/// <summary> Runs text through input replace, to convert to shorthand. </summary>
		public string InputToShorthand(string text)
		{
			if (inputReplace == null)
				return text;
			// we add spaces on the end so we can replace " u " with " you " and not replace " you " with " yoyou "
			string result = " " + SanitizeInputReplace(text) + " ";
			personControlLock.EnterReadLock();
			try
			{
				// Note: This method is not efficient at all.
				foreach (var level in inputReplace)
				{
					if (level == null)
						continue;

					text = result;
					foreach (var kvp in level)
					{
						int i = text.IndexOf(kvp.Key);
						if (i != -1)
						{
							result = result.Replace(kvp.Key, kvp.Value);
							text = text.Remove(i, kvp.Key.Length);
						}
					}
				}
			}
			finally
			{ personControlLock.ExitReadLock(); }
			// Remove spaces on the ends, then return.
			return result.Trim();
		}

		/// <summary> Parses rawLines as csv for the input replace system </summary>
		private void parseInputReplace(List<string> rawLines, Logger log)
		{
			for (int line = 0; line < rawLines.Count; ++line)
			{
				// get line ignore empty
				var str = rawLines[line];
				if (str == null || str.Length == 0 || str.StartsWith("//"))
					continue;
				log.SetId(line + 1);
				// split line, check length.
				var val = str.Split(',');
				if (val.Length == 0)
					continue;
				if (val.Length < 2)
				{
					log.Warning(StringsScripting.InputReplace_Pass_empty);
					continue;
				}
				// get pass and keyward.
				string keyword = SanitizeInputReplace(val[1]);
				int pass;
				if (!int.TryParse(val[0], out pass) || pass < 0)
				{
					log.Warning(StringsScripting.InputReplace_Pass_not_a_number);
					continue;
				}
				pass--;
				// fill list upto pass.
				while (pass >= inputReplace.Count)
				{
					inputReplace.Add(null);
				}
				if (inputReplace[pass] == null)
					inputReplace[pass] = new Dictionary<string, string>();
				// split, trim then add to level.
				var level = inputReplace[pass];
				for (int i = 2; i < val.Length; ++i)
				{
					var tmp = SanitizeInputReplace(val[i]);
					if (tmp.Length == 0)
						continue;
					if (tmp == keyword)
					{
						log.Warning(string.Format(StringsScripting.Formatted_InputReplace_Duplicate_keyword, keyword, val[i]));
						continue;
					}
					level[tmp] = keyword;
				}
			}
		}

		/// <summary> Sanitize strings for use in the input replace system. </summary>
		public static string[] SanitizeInputReplace(string[] strings)
		{
			return strings.Select(str => SanitizeInputReplace(str)).ToArray();
		}
		/// <summary> Sanitize a string for use in the input replace system. </summary>
		public static string SanitizeInputReplace(string str)
		{
			str = str.Trim().Trim(new char[] { '"' }).ToLowerInvariant();
			return new string(str.Where(c => !Punctuation.Contains(c)).ToArray());
		}

		#endregion

		/// <summary> Removes white-space, sets lowercase, removes invalid characters, trims white-space. </summary>
		public static string KeyClean(string key, Logger log = null)
		{
			bool tmp;
			return KeyClean(key, out tmp, out tmp, log);
		}
		/// <summary> Removes white-space, sets lowercase, removes invalid characters, trims white-space. </summary>
		public static string[] KeyClean(string[] keys, Logger log = null)
		{
			bool tmp;
			return keys.Select(key => KeyClean(key, out tmp, out tmp, log)).ToArray();
		}
		/// <summary> Removes white-space, sets lowercase, removes invalid characters, trims white-space. </summary>
		private static string KeyClean(string key, out bool isFloat, out bool isPercent, Logger log = null)
		{
			isPercent = false;
			isFloat = true;
			int start = 0;
			var array = key.ToCharArray();
			// ignore white-space at the start. Similar effect to TrimStart()
			for (; start < array.Length; ++start)
				if (!char.IsWhiteSpace(array[start]))
					break;
			// return if it was all white-space.
			if (start >= array.Length)
			{
				isFloat = false;
				return "";
			}

			int i = start;
			int end = start;
			char c;
			for (; i < array.Length; ++i)
			{
				c = array[i];
				if (char.IsWhiteSpace(c))
					array[i] = '_';
				else
				{
					end = i; // only set end when it's not white-space. Similar effect to TrimEnd()
					if (isFloat && !isNumberChar(c))
					{
						isFloat = false;
						if (c == '%')
						{
							isPercent = true;
							continue;
						}
					}
					else if (isPercent && !isNumberChar(c))
					{
						isPercent = false;
					}

					// allow . but not anything under 0 or betwen 9 and A
					if ((c < 48 && c != 46) || (c > 57 && c < 65))
					{
						array[i] = '_';
						if (log != null)
							log.Warning(string.Format(StringsScripting.Formatted_KeyClean_Invalid_character, c, key));
					}
					else if (c > 64)
						array[i] = char.ToLowerInvariant(c);
				}
			}
			if (isPercent) // precent has a non number char on the end, so ignore it.
				return new string(array, start, end - start);
			else
				return new string(array, start, end - start + 1);
		}
		/// <returns> true if char is dot .  or a ascii number. </returns>
		private static bool isNumberChar(char c)
		{
			return c == 46 || (c >= 48 && c <= 57);
		}
		/// <summary> Splits the key in two pices, first being root key, second is remaining key.
		public static string[] KeySplit(string key)
		{
			return key.Split(keySeparator, 2);
		}
	}
}
