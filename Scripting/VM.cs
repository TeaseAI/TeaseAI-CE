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
		private bool threadRun = false;

		private ReaderWriterLockSlim personControlLock = new ReaderWriterLockSlim();
		private Dictionary<string, Personality> personalities = new Dictionary<string, Personality>();
		private List<Controller> controllers = new List<Controller>();

		private ReaderWriterLockSlim scriptsLock = new ReaderWriterLockSlim();
		private Dictionary<string, Script> scripts = new Dictionary<string, Script>();

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
			Block tmpBlock = parseBlock(rawLines, ref currentLine, 0);
			if (tmpBlock == null)
				return false;
			// go thought each root block, and add it to the system.
			foreach (var line in tmpBlock.Lines)
			{
				if (line.SubBlock == null)
					continue;
				if (line.Data.StartsWith("script", StringComparison.InvariantCultureIgnoreCase))
				{
					scriptsLock.EnterWriteLock();
					try
					{
						scripts[line.Data.ToLowerInvariant()] = new Script(line.SubBlock.Lines);
					}
					finally
					{ scriptsLock.ExitWriteLock(); }

				}
				else
				{
					// ToDo : warning unknown root type.
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
		private Block parseBlock(List<string> rawLines, ref int currentLine, int blockIndent)
		{
			// temp list of lines, until we are finished parsing.
			var lines = new List<Block.Line>();

			string lineData;
			int lineIndent = 0;
			int indentDifference;
			// Note: This loop is picky, do NOT edit untilss you understand what is happening.
			// currentLine should be added to once we are done with the line.
			while (currentLine < rawLines.Count)
			{
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
						var subBlock = parseBlock(rawLines, ref currentLine, lineIndent);
						if (lines.Count == 0)
						{
							// ToDo : Warning invalid indatation. (not sure if this error is even possible.)
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
						// ToDo : Error invalid indentation.
						++currentLine;
					}
				}
				else // line was empty
					++currentLine;
			}

			if (lines.Count > 0)
				return new Block(lines.ToArray());
			return null;
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
			var key = name.ToLowerInvariant();
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

		public Script GetScript(string name)
		{
			var key = name.ToLowerInvariant();
			Script result = null;
			scriptsLock.EnterReadLock();
			try
			{
				result = scripts[key];
			}
			finally
			{
				scriptsLock.ExitReadLock();
			}
			return result;
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
	}
}
