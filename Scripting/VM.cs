using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// Main 'world' object, needs to know about all scripts, personalities, controllers, functions, etc..
	/// </summary>
	public class VM
	{
		public delegate Variable Function(BlockScope sender, Variable[] args);

		private Thread thread = null;
		private volatile bool threadRun = false;
		public bool IsRunning { get { return threadRun; } }

		private ConcurrentDictionary<string, Function> functions = new ConcurrentDictionary<string, Function>();

		private ReaderWriterLockSlim personControlLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		private Dictionary<string, Personality> personalities = new Dictionary<string, Personality>();
		private List<Controller> controllers = new List<Controller>();
		private List<Dictionary<string, string>> inputReplace;

		private ReaderWriterLockSlim scriptsLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		private List<GroupInfo> scriptGroups = new List<GroupInfo>();
		private List<Script> scriptSetups = new List<Script>();
		private Dictionary<string, Variable<Script>> scripts = new Dictionary<string, Variable<Script>>();

		public const char IndentChar = '\t';
		public static readonly char[] InvalidKeyChar = new char[] { '#', '@', '(', ',', ')', '"', '\\', '/', '*', '+', '-', '<', '=', '>' };
		public static readonly char[] Punctuation = new char[] { '\'', '!', '?', '.', ',' };
		private static readonly char[] keySeparator = { '.' };

		#region Tick thread
		/// <summary> Starts the controller update thread </summary>
		public void Start()
		{
			if (thread == null || !thread.IsAlive)
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
			while (threadRun)
			{
				// update all controllers
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
				Thread.Sleep(50);
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
			personControlLock.EnterWriteLock();
			try
			{
				// If personality with id already exists, add number.
				if (personalities.ContainsKey(id))
				{
					int num = 1;
					while (personalities.ContainsKey(id + (++num)))
					{ }
					id = id + num;
				}

				var p = new Personality(this, name, id);
				personalities[id] = p;
				return p;
			}
			finally
			{ personControlLock.ExitWriteLock(); }
		}

		public bool TryGetPersonality(string name, out Personality p)
		{
			personControlLock.EnterReadLock();
			try
			{
				return personalities.TryGetValue(KeyClean(name), out p);
			}
			finally
			{ personControlLock.ExitReadLock(); }
		}

		public bool ChangePersonalityID(Personality p, string newID)
		{
			newID = KeyClean(newID);
			personControlLock.EnterWriteLock();
			try
			{
				if (personalities.ContainsKey(newID))
					return false;
				personalities.Remove(p.ID);
				personalities[newID] = p;
				p.ID = newID;
				return true;
			}
			finally
			{ personControlLock.ExitWriteLock(); }
		}

		/// <summary>
		/// Creates a new controller for the given personality.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public Controller CreateController(Personality p)
		{
			personControlLock.EnterWriteLock();
			try
			{
				var c = new Controller(p);
				controllers.Add(c);
				return c;
			}
			finally
			{ personControlLock.ExitWriteLock(); }
		}

		public Personality[] GetPersonalities()
		{
			personControlLock.EnterReadLock();
			try
			{
				return personalities.Values.ToArray();
			}
			finally
			{ personControlLock.ExitReadLock(); }
		}

		#endregion

		internal Variable GetVariable(string key, BlockScope sender)
		{
			// allow calling local script without full key.
			if (key.StartsWith("script."))
			{
				var tmpK = sender.Root.Group.Key + '.' + KeySplit(key)[1];
				scriptsLock.EnterReadLock();
				try
				{
					Variable<Script> result;
					if (scripts.TryGetValue(tmpK, out result))
						return result;
				}
				finally
				{ scriptsLock.ExitReadLock(); }
			}
			return GetVariable(key, sender.Root.Log);
		}
		/// <summary>
		/// Get a variable, or null if not found.
		/// </summary>
		/// <param name="key">Clean key</param>
		/// <param name="log"></param>
		/// <returns></returns>
		internal Variable GetVariable(string key, Logger log)
		{
			var keySplit = KeySplit(key);

			if (keySplit.Length == 1)
			{
				switch (keySplit[0])
				{
					case "script":
					case "personality":
						log.Error("No " + keySplit[0] + " specified!");
						return null;
				}
			}

			switch (keySplit[0])
			{
				case "script":
					scriptsLock.EnterReadLock();
					try
					{
						Variable<Script> result;
						if (scripts.TryGetValue(keySplit[1], out result))
						{
							if (result.IsSet && result.Value.Valid == BlockBase.Validation.Failed)
							{
								log.Error(string.Format("Requested script '{0}' failed valadation!", keySplit[1]));
								return null;
							}
						}
						else
							log.Error("Script not found: " + keySplit[1]);
						return result;
					}
					finally
					{ scriptsLock.ExitReadLock(); }

				// return personality or a variable from the personailty.
				case "personality":
					var pKey = KeySplit(keySplit[1]); // split key into [0]personality key, [1]variable key
					personControlLock.EnterReadLock();
					try
					{
						Personality p;
						if (personalities.TryGetValue(pKey[0], out p))
						{
							if (pKey.Length == 2) // return variable if we have key.
								return p.getVariable_internal(pKey[1]);
							// else just return the personality.
							return new Variable<Personality>(p);
						}
						log.Error("Personality not found: " + pKey[0]);
						return null;
					}
					finally
					{ personControlLock.ExitReadLock(); }



				default: // Function?
					Function func;
					if (functions.TryGetValue(key, out func))
						return new Variable<Function>(func);
					else
						log.Error("Function not found or bad namespace: " + keySplit[0]);
					return null;
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
			var c = new Controller(p);
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

		#region Loading files

		/// <summary>
		/// Load files in path and all sub directories.
		/// </summary>
		public void LoadFromDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				(new Logger(path)).Error("Directory does not exist!");
				return;
			}

			// load all input replace files.
			var files = Directory.GetFiles(path, "input replace.csv", SearchOption.AllDirectories);
			personControlLock.EnterWriteLock();
			try
			{
				inputReplace = new List<Dictionary<string, string>>();
				foreach (var file in files)
				{
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
			int blockLine = 0;
			while (currentLine < rawLines.Count)
			{
				string str = rawLines[currentLine];
				int indent = 0;
				if (parseCutLine(ref str, ref indent)) // line empty?
				{
					if (indent == 0) // indent 0 defines what the key of the upcoming object is.
					{
						blockKey = KeyClean(str);
						blockLine = currentLine;
						// make sure key is a valid type.
						var rootKey = KeySplit(blockKey)[0];
						if (rootKey != "script" && rootKey != "list" && rootKey != "setup" && rootKey != "personality")
						{
							fileLog.Error("Invalid root type: " + rootKey, currentLine);
							break;
						}
						++currentLine;
					}
					else if (indent > 0)
					{
						if (blockKey == null)
						{
							fileLog.Error("Invalid indentation!", currentLine);
							break;
						}
						var log = new Logger(fileKey + "." + blockKey);
						var lines = parseBlock(rawLines, ref currentLine, indent, log);
						if (lines == null)
							blocks.Add(new BlockBase(blockLine, blockKey, null, group, log));
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
									{ scriptSetups.Add(new Script(blockLine, blockKey, lines, group, log)); }
									finally
									{ scriptsLock.ExitWriteLock(); }
								}
								else
								{
									fileLog.Error("Invalid root type: " + keySplit[0], blockLine);
									break;
								}
							}
							else if (keySplit.Length == 2)
							{
								if (keySplit[1] == null || keySplit[1].Length == 0)
								{
									fileLog.Warning("sub key has length of zero!");
									continue;
								}
								var key = fileKey + '.' + keySplit[1];
								switch (keySplit[0])
								{
									case "script":
										scriptsLock.EnterWriteLock();
										try
										{
											var script = new Script(blockLine, blockKey, lines, group, log);
											blocks.Add(script);
											scripts[key] = new Variable<Script>(script);
										}
										finally
										{ scriptsLock.ExitWriteLock(); }
										break;
									case "list":
										scriptsLock.EnterWriteLock();
										try
										{ } // ToDo : List
										finally
										{ scriptsLock.ExitWriteLock(); }
										break;
									case "personality":
										{
											key = keySplit[1];
											// does personality already exist?
											Personality p = null;
											personControlLock.EnterReadLock();
											personalities.TryGetValue(key, out p);
											personControlLock.ExitReadLock();
											// if not create new.
											if (p == null)
												p = CreatePersonality(key);
											// run through the script to fill the personalities variables.
											var script = new Script(blockLine, blockKey, lines, group, log);
											var c = new Controller(p);
											validateScript(c, script);
											runThroughScript(p, c, script, new StringBuilder());
										}
										break;
									default:
										fileLog.Error("Invalid root type: " + keySplit[0], blockLine);
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
		private Line[] parseBlock(List<string> rawLines, ref int currentLine, int blockIndent, Logger log)
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
				log.SetId(currentLine);
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
						lines.Add(new Line(currentLine, lineData, null));
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
							log.Warning("Invalid indatation. (not sure if this error is even possible.)");
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
						log.Warning("Invalid indentation.");
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
		private bool parseCutLine(ref string str, ref int indentCount)
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

		#endregion

		#region Validation

		public void ValidateScripts()
		{
			var p = new Personality(this, "tmpValidator", "tmpValidator");
			var c = new Controller(p);

			scriptsLock.EnterReadLock();
			try
			{
				// validate startup scripts.
				foreach (var s in scriptSetups)
					validateScript(c, s);

				// validate all other scripts.
				foreach (var s in scripts.Values)
					if (s.IsSet)
						validateScript(c, s.Value);
			}
			finally
			{ scriptsLock.ExitReadLock(); }
		}
		private void validateScript(Controller c, Script s)
		{
			// if script has never been validated, but has errors, then do not validate, they are parse errors.
			if (s.Valid == BlockBase.Validation.NeverRan && s.Log.ErrorCount > 0)
			{
				s.SetValid(false); // will set valid to failed.
				return;
			}

			s.SetValid(true);
			c.Add(s);
			var sb = new StringBuilder();
			while (c.next(sb))
			{

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
		internal void ExecLine(BlockScope sender, string line, StringBuilder output)
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
							Variable variable = sender.GetVariable(key);
							if (variable == null || variable.IsSet == false)
								continue;
							object value = variable.Value;
							var func = value as Function;
							if (func != null)
								value = func(sender, args);
							// ToDo : Do we need to do anything to other value types?

							// output if @
							if (c == '@' && value != null)
								output.Append(value.ToString());
						}
						else
						{
							if (c == '@' && args != null && args.Length > 0 && args[0] != null)
								output.Append(args[0].ToString());
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
		private void execSplitCommand(BlockScope sender, string str, ref int i, out string key, out Variable[] args)
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
				key = KeyClean(sb.ToString());
				KeyIsValid(sender.Root.Log, key);
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
		private Variable[] execParentheses(BlockScope sender, string str, ref int i)
		{
			var log = sender.Root.Log;
			int start = i;
			var outArgs = new List<Variable>();
			outArgs.Add(null); // Set to the output before return.
			var items = new List<execParenthItem>();

			// Split in to lists of values and operators.
			var sb = new StringBuilder();
			bool inString = false;
			bool finished = false;
			char c;
			while (i < str.Length && !finished)
			{
				c = str[i];
				if (inString)
				{
					if (c == '"') // string end
					{
						items.Add(new Variable(sb.ToString()));
						sb.Clear();
						inString = false;
					}
					// escape
					else if (c == '\\' && i + 1 < str.Length)
					{
						++i;
						char next = str[i];
						if (next == '\\')
							sb.Append('\\');
						else if (next == 'n')
							sb.Append('\n');
						else if (next == '"')
							sb.Append('"');
						else
							log.Warning("Invalid string escape character: " + next, -1, i);
					}
					else
						sb.Append(c);
					++i;
					continue;
				}
				//else not in string
				switch (c)
				{
					case '"': // string start
						execParenthCheckAdd(sender, items, sb);
						inString = true;
						break;

					// finish parenth
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
								funcParenth = v.IsSet && v is Variable<Function>;
							}

							++i;
							var _char = i;
							var args = execParentheses(sender, str, ref i);

							if (funcParenth) // simply add the whole array as-is.
								items.Add(new Variable<Variable[]>(args));
							else if (args.Length == 0)
								log.Warning("Empty sub parentheses", -1, _char);
							else
								items.Add(args[0]); // sense it's not for a function, we only care about the first arg.

							if (args.Length > 1)
								log.Warning("Sub parentheses have more than one argument!", -1, _char);
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
				log.Warning("Parentheses is empty!");
				return new Variable[0];
			}
			else if (items[0].IsValue == false)
			{
				log.Error("Parentheses must start with a variable/value, not an operator.");
				return new Variable[0];
			}

			// Evaluate
			int j;

			// Operator precedence
			// 1. Parentheses          ( )
			// 2. Execute functions
			// 3. Multiply & Divide    * /
			// 4. Add & Subtract       + -
			// 5. logic 1              > < ==
			// 6. logic 2             and or
			// 7. Assignment          =
			//Note: assignment goes right to left.

			// 1. Done when we parse
			// 2. Execute functions
			j = 0;
			for (; j < items.Count; ++j)
			{
				if (items[j].IsValue)
				{
					var func = items[j].Value as Variable<Function>;
					if (func == null)
						continue;
					Variable[] args = null;
					// check if args is next item
					if (j + 1 < items.Count && items[j + 1].IsValue)
					{
						var argvar = items[j + 1].Value as Variable<Variable[]>;
						if (argvar == null)
						{
							log.Error("Expecting function arguments.");
							return new Variable[0];
						}
						args = argvar.Value;
						items.RemoveAt(j + 1);
					}
					if (args == null)
						args = new Variable[0];
					items[j] = func.Value(sender, args);
				}
			}

			// At this point it is required that there is exactly one operator inbetween each variable.
			j = 0;
			while (j + 2 < items.Count)
			{
				var l = items[j];
				var o = items[j + 1];
				var r = items[j + 2];
				if (l.IsValue == false || r.IsValue == false)
				{
					log.Error("Expecting variable/value, but got an operator.");
					return new Variable[0];
				}
				else if (o.IsValue)
				{
					log.Error("Expecting a operator, but got a variable/value.");
					return new Variable[0];
				}
				j += 2;
			}

			// 3. Multiply & Divide
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
			// 4. Add & Subtract
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
			// 5. logic 1
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
			// 6. logic 2
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
			// 7. Assignment
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
		private bool execParenthCheckAdd(BlockScope sender, List<execParenthItem> items, StringBuilder sb)
		{
			if (sb.Length == 0)
				return false;

			string str = sb.ToString().Trim().ToLowerInvariant();
			sb.Clear();
			float f;
			bool b;
			// is str float?
			if (float.TryParse(str, out f))
				items.Add(new Variable(f));
			// percent to float?
			else if (str.EndsWith("%") && float.TryParse(str.Substring(0, str.Length - 1), out f))
				items.Add(new Variable(f * 0.01f));
			// bool
			else if (bool.TryParse(str, out b))
				items.Add(new Variable(b));
			// logic operators
			else if (str == "and")
				items.Add(Operators.And);
			else if (str == "or")
				items.Add(Operators.Or);
			else // variable
			{
				string key = KeyClean(str);
				if (!KeyIsValid(sender.Root.Log, key))
					return false;
				var variable = sender.GetVariable(key);
				items.Add(variable);
			}
			return true;
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
				if (str == null || str.Length == 0 || line == 0)
					continue;
				log.SetId(line + 1);
				// split line, check length.
				var val = str.Split(',');
				if (val.Length == 0)
					continue;
				if (val.Length < 2)
				{
					log.Warning("Pass is required!");
					continue;
				}
				// get pass and keyward.
				string keyword = SanitizeInputReplace(val[1]);
				int pass;
				if (!int.TryParse(val[0], out pass) || pass < 0)
				{
					log.Warning("Invalid level :" + val[0]);
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
						log.Warning(string.Format("'{0}' is not needed because it is equal to the keyword '{1}'", val[i], keyword));
						continue;
					}
					level[tmp] = keyword;
				}
			}
		}

		/// <summary> Sanitize a string for use in the input replace system. </summary>
		public static string SanitizeInputReplace(string str)
		{
			str = str.Trim().Trim(new char[] { '"' }).ToLowerInvariant();
			return new string(str.Where(c => !Punctuation.Contains(c)).ToArray());
		}

		#endregion

		/// <summary> Removes white-space, sets lowercase, removes invalid characters. </summary>
		public static string KeyClean(string key)
		{
			var array = key.ToCharArray();
			char c;
			for (int i = 0; i < array.Length; ++i)
			{
				c = array[i];
				if (char.IsWhiteSpace(c))
					array[i] = '_';
				else
					array[i] = char.ToLowerInvariant(c);
			}
			return new string(array);
		}
		/// <summary> Splits the key in two pices, first being root key, second is remaining key.
		public static string[] KeySplit(string key)
		{
			return key.Split(keySeparator, 2);
		}
		/// <summary> true if key does not contain any InvalidKeyChar, logs error and returns false otherwise. </summary>
		public static bool KeyIsValid(Logger log, string key)
		{
			if (InvalidKeyChar.Any(c => key.Contains(c)))
			{
				log.Error("Key contains some invalid character(s).");
				return false;
			}
			return true;
		}
	}
}
