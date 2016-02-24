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

		public void SetId(int line, int @char = 0)
		{
			if (line != -1)
				id_line = line;
			id_char = @char;
		}

		public int ErrorCount { get { return errors.Count; } }
		public void Error(string message)
		{
			log(message, Level.Error);
		}
		public void Error(string message, int line, int @char = 0)
		{
			SetId(line, @char);
			log(message, Level.Error);
		}
		public void Warning(string message)
		{
			log(message, Level.Warning);
		}
		public void Warning(string message, int line, int @char = 0)
		{
			SetId(line, @char);
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

		public void Clear()
		{
			SetId(0, 0);
			errors.Clear();
			warnings.Clear();
		}
	}
}
