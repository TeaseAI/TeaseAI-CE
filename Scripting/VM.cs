using System;
using System.Collections.Generic;
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
		public delegate ValueObj Function(BlockScope sender, ValueObj[] args, Logger log);

		private Thread thread = null;
		private volatile bool threadRun = false;

		private ReaderWriterLockSlim personControlLock = new ReaderWriterLockSlim();
		private Dictionary<string, Personality> personalities = new Dictionary<string, Personality>();
		private List<Controller> controllers = new List<Controller>();

		private ReaderWriterLockSlim scriptsLock = new ReaderWriterLockSlim();
		private List<Script> scriptSetups = new List<Script>();
		private Dictionary<string, ValueScript> scripts = new Dictionary<string, ValueScript>();

		public const char IndentChar = '\t';

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

		/// <summary>
		/// Craete a new personality with given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns>New Personality or null if name already exits.</returns>
		public Personality CreatePersonality(string name)
		{
			var key = KeyClean(name);
			personControlLock.EnterWriteLock();
			try
			{
				if (personalities.ContainsKey(key))
				{
					// ToDo : Error personality with name already exists.
					return null;
				}

				var p = new Personality(this, name, key);
				personalities[key] = p;
				return p;
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

		/// <summary>
		/// Get a variable, or null if not found.
		/// </summary>
		/// <param name="key">Clean key</param>
		/// <param name="log"></param>
		/// <returns></returns>
		internal ValueObj GetVariable(string key, Logger log)
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
						ValueScript result;
						if (!scripts.TryGetValue(keySplit[1], out result))
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
								return p.getVariable_internal(pKey[1], log);
							else // else just return the personality.
								return new ValuePersonality(p);
						}
						log.Error("Personality not found: " + pKey[0]);
						return null;
					}
					finally
					{ personControlLock.ExitReadLock(); }



				default: // Function?
					log.Error("Function not found or bad namespace: " + keySplit[0]);
					return null;
			}
		}

		#region Loading and parsing files

		/// <summary>
		/// Load scripts with .vtscript in path and all sub directories.
		/// </summary>
		/// <param name="path"></param>
		public void LoadScripts(string path)
		{
			if (!Directory.Exists(path))
			{
				// ToDo : Log directory does not exist.
				return;
			}

			var infos = new List<GroupInfo>();

			var files = Directory.GetFiles(path, "*.vtscript", SearchOption.AllDirectories);
			foreach (string file in files)
				infos.Add(parseFile(file));
		}

		/// <summary>
		/// Loads file from disk and parses in to the system.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		private GroupInfo parseFile(string file)
		{
			var fileLog = new Logger();
			// get the base script key from the file name.
			var fileKey = KeyClean(Path.GetFileNameWithoutExtension(file));

			if (!File.Exists(file))
			{
				fileLog.Error("File does not exist!");
				return new GroupInfo(file, fileKey, fileLog, null);
			}

			// Read the whole file into a tempary list.
			var rawLines = new List<string>();
			try
			{
				using (var sr = new StreamReader(file))
					while (!sr.EndOfStream)
						rawLines.Add(sr.ReadLine());
			}
			catch (Exception ex)
			{
				fileLog.Error(ex.Message);
				return new GroupInfo(file, fileKey, fileLog, null);
			}

			// split-up the lines in to indatation based blocks.
			var blocks = new List<BlockBase>();
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
						if (rootKey != "script" && rootKey != "list" && rootKey != "setup")
						{
							fileLog.Error(currentLine, "Invalid root type: " + rootKey);
							break;
						}
						++currentLine;
					}
					else if (indent > 0)
					{
						if (blockKey == null)
						{
							fileLog.Error(currentLine, "Invalid indentation!");
							break;
						}
						var log = new Logger();
						var lines = parseBlock(rawLines, ref currentLine, indent, log);
						if (lines == null)
							blocks.Add(new BlockBase(blockLine, blockKey, null, log));
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
									{ scriptSetups.Add(new Script(blockLine, blockKey, lines, log)); }
									finally
									{ scriptsLock.ExitWriteLock(); }
								}
								else
								{
									fileLog.Error(blockLine, "Invalid root type: " + keySplit[0]);
									break;
								}
							}
							else if (keySplit.Length == 2)
							{
								var key = fileKey + '.' + keySplit[1];
								switch (keySplit[0])
								{
									case "script":
										scriptsLock.EnterWriteLock();
										try
										{
											var script = new Script(blockLine, blockKey, lines, log);
											blocks.Add(script);
											scripts[key] = new ValueScript(script);
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
									default:
										fileLog.Error(blockLine, "Invalid root type: " + keySplit[0]);
										return new GroupInfo(file, fileKey, fileLog, blocks.ToArray());
								}
							}
						}
					}
				}
				else
					++currentLine;
			}
			return new GroupInfo(file, fileKey, fileLog, blocks.ToArray());
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

		#region executing lines

		internal void ExecLine(BlockScope sender, string line, StringBuilder output)
		{
			var log = sender.Root.Log;
			string key;
			ValueObj[] args;

			int i = 0;
			char c;
			while (i < line.Length)
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
							ValueObj variable = sender.GetVariable(key);
							if (variable == null)
								return;
							if (variable is ValueFunction)
								variable = ((ValueFunction)variable).Value(sender, args, sender.Root.Log);
							// ToDo : Do we need to do anything to other value types?

							// output if @
							if (c == '@' && variable != null)
								output.Append(variable.ToString());
						}
						else
						{
							if (c == '@' && args != null && args.Length > 0 && args[0] != null)
								output.Append(args[0].ToString());
						}
						break;

					case '\\':
						// ToDo : escape character.
						break;
					default:
						output.Append(c);
						break;
				}
			}
		}
		private void execSplitCommand(BlockScope sender, string str, ref int i, out string key, out ValueObj[] args)
		{
			args = null;
			var sb = new StringBuilder();
			char c;
			while (i < str.Length)
			{
				c = str[i];
				++i;
				if (c == ' ')
					break;
				else if (c == '(')
				{
					args = execParentheses(sender, str, ref i);
					break;
				}
				sb.Append(c);
			}
			if (sb.Length > 0)
				key = KeyClean(sb.ToString());
			else
				key = null;
			if (args == null)
				args = new ValueObj[0];
		}

		private ValueObj[] execParentheses(BlockScope sender, string str, ref int i)
		{
			// ToDo : Parentheses
			return null;
		}

		#endregion

		/// <summary>
		/// Removes white-space, sets lowercase, removes invalid characters.
		/// </summary>
		public static string KeyClean(string key)
		{
			var array = key.ToCharArray();
			char c;
			for (int i = 0; i < array.Length; ++i)
			{
				c = array[i];
				if (char.IsWhiteSpace(c))
					array[i] = '_';
				//else if (c == '#' || c == '@')
				//	array[i] = '?';
				else
					array[i] = char.ToLowerInvariant(c);
			}
			return new string(array);
		}
		private readonly static char[] keySeparator = { '.' };
		public static string[] KeySplit(string key)
		{
			return key.Split(keySeparator, 2);
		}
	}
}
