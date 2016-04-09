using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TeaseAI_CE.Scripting
{
	public class Logger
	{
		public enum Level { Error, Warning, Info }

		private string prefix;

		private List<string> errors = new List<string>();
		private List<string> warnings = new List<string>();

		private int id_line;
		private int id_char;

		public Logger(string prefix)
		{
			this.prefix = prefix;
		}

		public void SetId(int line, int @char = 0)
		{
			if (line != -1)
				id_line = line;
			id_char = @char;
		}

		public int ErrorCount { get { return errors.Count; } }
		public void Error(string message)
		{
			Log(this, Level.Error, message);
		}
		public void ErrorF(string format, params object[] args)
		{
			LogF(this, Level.Error, format, args);
		}
		public void Error(string message, int line, int @char = 0)
		{
			SetId(line, @char);
			Log(this, Level.Error, message);
		}
		public void Warning(string message)
		{
			Log(this, Level.Warning, message);
		}
		public void WarningF(string format, params object[] args)
		{
			LogF(this, Level.Warning, format, args);
		}
		public void Warning(string message, int line, int @char = 0)
		{
			SetId(line, @char);
			Log(this, Level.Warning, message);
		}
		public void Info(string message)
		{
			log_global(prefix, message, Level.Info);
		}

		public void Clear()
		{
			SetId(0, 0);
			errors.Clear();
			warnings.Clear();
		}

		public static void LogF(Logger log, Level level, string format, params object[] args)
		{
			string message;
			try
			{ message = string.Format(format, args); }
			catch
			{ message = "Logging format error: " + format; }
			Log(log, level, message);
		}
		public static void Log(Logger log, Level level, string message)
		{
			if (log != null)
			{
				string str = string.Format("[{0,2}, {1,2}]: {2}", log.id_line, log.id_char, message);
				if (level == Level.Error)
					log.errors.Add(level.ToString() + message);
				else if (level == Level.Warning)
					log.warnings.Add(level.ToString() + message);
				log_global(log.prefix, str, level);
			}
			else
				log_global("-global-", message, level);
		}
		private static void log_global(string prefix, string str, Level level)
		{
			Trace.WriteLine(string.Format("{0,-7}[{1}]{2}", level, prefix, str));
		}

	}
	/// <summary>
	/// Logs start message on creation, then end message with the elapsed time on dispose.
	/// </summary>
	public class LogTimed : IDisposable
	{
		public string EndMessage;
		private Stopwatch timmer;
		private Logger log;
		public LogTimed(Logger log, string startMessage, string endMessage = null)
		{
			this.log = log;

			if (startMessage != null && startMessage.Length != 0)
				log.Info(startMessage);
			if (endMessage != null)
				EndMessage = endMessage;
			else
				EndMessage = startMessage;

			timmer = new Stopwatch();
			timmer.Start();
		}
		public void Dispose()
		{
			if (timmer != null)
			{
				timmer.Stop();
				log.Info(EndMessage + " in " + timmer.ElapsedMilliseconds + "ms");
				log = null;
				timmer = null;
			}
		}
	}
}
