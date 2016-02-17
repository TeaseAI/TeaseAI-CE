using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace TeaseAI_CE.Scripting
{
	public class VM
	{
		private Thread thread = null;
		private volatile bool threadRun = false;

		private ReaderWriterLockSlim personControlLock = new ReaderWriterLockSlim();
		private Dictionary<string, Personality> personalities = new Dictionary<string, Personality>();
		private List<Controller> controllers = new List<Controller>();

		private ReaderWriterLockSlim scriptsLock = new ReaderWriterLockSlim();
		private List<Script> scriptSetups = new List<Script>();
		private Dictionary<string, ValueScript> scripts = new Dictionary<string, ValueScript>();

		public const char IndentChar = '\t';


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

			var files = Directory.GetFiles(path, "*.vtscript", SearchOption.AllDirectories);
			foreach (string file in files)
			{
				if (!parseFile(file))
				{
					// ToDo : Log file unable to load.
				}
			}
		}

		/// <summary>
		/// Loads file from disk and parses in to the system.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		private bool parseFile(string file)
		{
			if (!File.Exists(file))
			{
				// ToDo : Log file does not exist.
				return false;
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
				// ToDo : Log file read error.
				System.Diagnostics.Debug.WriteLine(ex.Message);
				return false;
			}

			// split-up the lines in to indatation based blocks.
			int currentLine = 0;
			Block tmpBlock = parseBlock(rawLines, ref currentLine, 0, null, true);
			if (tmpBlock == null)
				return false;

			// get the base script key from the file name.
			var fileKey = KeyClean(Path.GetFileNameWithoutExtension(file));
			// ToDo : script file info class.
			// somthing that has a list of all the scripts that where in the file.

			// go thought each root block, and add it to the system.
			foreach (var line in tmpBlock.Lines)
			{
				BlockBase block = line.SubBlock as BlockBase;
				if (block == null)
					continue;
				var keySplit = KeySplit(KeyClean(line.Data));
				if (keySplit.Length == 1)
				{
					if (keySplit[0] == "setup")
					{
						scriptsLock.EnterWriteLock();
						try
						{ scriptSetups.Add(new Script(block)); }
						finally
						{ scriptsLock.ExitWriteLock(); }
					}
					else
					{
						// ToDo : warning unknown root type.
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
							{ scripts[key] = new ValueScript(new Script(block)); }
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
							// ToDo : warning unknown root type.
							break;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Parses rawLines in to blocks of code recursively based on blockIndent.
		/// </summary>
		/// <param name="rawLines"></param>
		/// <param name="currentLine"></param>
		/// <param name="blockIndent">Indent level this block is at.</param>
		/// <returns>Block with lines, or null if zero lines.</returns>
		private Block parseBlock(List<string> rawLines, ref int currentLine, int blockIndent, Logger log, bool isRoot)
		{
			// temp list of lines, until we are finished parsing.
			var lines = new List<Block.Line>();

			bool isBase = log == null || isRoot;
			if (log == null)
				log = new Logger();

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
						lines.Add(new Block.Line(currentLine, lineData, null));
						++currentLine;
					}
					// next level of indentation. Parse as a sub block, then add to last line.
					else if (indentDifference == +1)
					{
						Block subBlock;
						if (isRoot)
							subBlock = parseBlock(rawLines, ref currentLine, lineIndent, null, false);
						else
							subBlock = parseBlock(rawLines, ref currentLine, lineIndent, log, false);
						if (lines.Count == 0)
						{
							log.Warning("Invalid indatation. (not sure if this error is even possible.)");
							lines.Add(new Block.Line(-1, "", subBlock));
						}
						else
						{
							var tmp = lines[lines.Count - 1];
							lines[lines.Count - 1] = new Block.Line(tmp.LineNumber, tmp.Data, subBlock);
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
			if (isBase)
				return new BlockBase(log, lines.ToArray());
			else
				return new Block(lines.ToArray());
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

				case "personality":
					personControlLock.EnterReadLock();
					try
					{
						Personality result;
						if (personalities.TryGetValue(keySplit[1], out result))
							return new ValuePersonality(result);
						else
							log.Error("Personality not found: " + keySplit[1]);
						return null;
					}
					finally
					{ personControlLock.ExitReadLock(); }



				default: // Function?
					log.Error("Function not found or bad namespace: " + keySplit[0]);
					return null;
			}
		}

		public void Start()
		{
			if (thread == null || !thread.IsAlive)
			{
				threadRun = true;
				thread = new Thread(threadTick);
				thread.Start();
			}
		}
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
