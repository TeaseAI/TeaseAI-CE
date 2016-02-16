using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	public class BlockBase : Block
	{
		public enum Validation { NeverRan = 0, Running, Passed, Failed }
		public Logger Log = new Logger();

		public Validation Valid { get; private set; }

		public BlockBase(BlockBase copy) : this(copy.Log, copy.Lines) { }
		public BlockBase(Logger log, Line[] lines) : base(lines)
		{
			Log = log;
			Valid = Validation.NeverRan;
		}

		/// <summary>
		/// Sets valid to running, or to passed/failed.
		/// </summary>
		/// <param name="running">If false, determin whether it passed.</param>
		public void SetValid(bool running)
		{
			if (running)
				Valid = Validation.Running;
			else
			{
				if (Log.ErrorCount == 0)
					Valid = Validation.Passed;
				else
					Valid = Validation.Failed;
			}
		}

		public class Logger
		{
			private enum Level { Error, Warning }

			private List<string> errors = new List<string>();
			private List<string> warnings = new List<string>();

			private int id_line;
			private int id_char;

			public void SetId(int line, int @char)
			{
				id_line = line;
				id_char = @char;
			}

			public int ErrorCount { get { return errors.Count; } }
			public void Error(string message)
			{
				log(message, Level.Error);
			}
			public void Warning(string message)
			{
				log(message, Level.Warning);
			}
			private void log(string message, Level level)
			{
				string str = level.ToString() + "[" + id_line + "][" + id_char + "]: " + message;
				if (level == Level.Error)
					errors.Add(message);
				else if (level == Level.Warning)
					warnings.Add(message);
				// ToDo 4: Link with program logger, to write logs to file.
				System.Diagnostics.Debug.WriteLine("Script log: " + str);
			}
		}
	}
}
