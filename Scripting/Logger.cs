using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	public class Logger
	{
		private enum Level { Error, Warning }

		private List<string> errors = new List<string>();
		private List<string> warnings = new List<string>();

		private int id_line;
		private int id_char;

		public void SetId(int line)
		{
			id_line = line;
			id_char = 0;
		}
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
		public void Error(int line, string message)
		{
			SetId(line);
			log(message, Level.Error);
		}
		public void Warning(string message)
		{
			log(message, Level.Warning);
		}
		public void Warning(int line, string message)
		{
			SetId(line);
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
			System.Diagnostics.Debug.WriteLine("### Scripting: " + str);
		}
	}
}
