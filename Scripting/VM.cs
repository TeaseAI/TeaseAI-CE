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
		// ToDo : Proper thread safety. (there are _many_ holes)
		private Thread thread;
		private bool threadRun = false;

		private List<Personality> personalities = new List<Personality>();
		private List<Controller> controllers = new List<Controller>();

		private ReaderWriterLockSlim scriptsLock = new ReaderWriterLockSlim();
		private Dictionary<string, Script> scripts = new Dictionary<string, Script>();

		public const char IndentChar = '\t';

		public VM()
		{
			thread = new Thread(new ThreadStart(threadTick));
		}

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
				parseFile(file);
			}
		}

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
					{
						scriptsLock.ExitWriteLock();
					}

				}
				else
				{
					// ToDo : warning unknown root type.
				}
			}
			return true;
		}

		private Block parseBlock(List<string> rawLines, ref int currentLine, int blockIndent)
		{
			var lines = new List<Block.Line>();

			string lineData;
			int lineIndent = 0;
			int indentDifference;
			while (currentLine < rawLines.Count)
			{
				lineData = rawLines[currentLine];
				if (parseCutLine(ref lineData, ref lineIndent))
				{
					indentDifference = lineIndent - blockIndent;
					if (indentDifference < 0)
					{
						break;
					}
					else if (indentDifference == +1)
					{
						int lineIndex = lines.Count - 1;
						var subBlock = parseBlock(rawLines, ref currentLine, lineIndent);

						if (lineIndex < 0)
						{
							// ToDo : Warning invalid indatation.
							lines.Add(new Block.Line(-1, "", subBlock));
						}
						else
						{
							var tmp = lines[lineIndex];
							lines[lineIndex] = new Block.Line(tmp.LineNumber, tmp.Data, subBlock);
						}
					}
					else if (indentDifference == 0) // indentation unchanged, so just add the line.
					{
						lines.Add(new Block.Line(currentLine, lineData, null));
						++currentLine;
					}
					else // invalid indentation.
					{
						// ToDo : Error invalid indentation.
						++currentLine;
					}
				}
				else
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

		public void Start()
		{
			threadRun = true;
			if (!thread.IsAlive)
				thread.Start();
			// ToDo : Add proper checks for thread start and stop.
		}
		public void Stop()
		{
			threadRun = false;
			if (thread.IsAlive)
				thread.Abort();
		}


		public Personality CreatePersonality()
		{
			var p = new Personality();

			personalities.Add(p);
			return p;
		}

		public Controller CreateController(Personality p)
		{
			var c = new Controller(this, p);
			controllers.Add(c);
			return c;
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
