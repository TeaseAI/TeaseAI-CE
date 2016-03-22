using System;
using System.IO;
using System.Diagnostics;

namespace TeaseAI_CE
{
	/// <summary>
	/// Writes Trace to log.txt, with a backup file log_old.txt
	/// </summary>
	class TraceToFileListener : TraceListener, IDisposable
	{
		private const string file = "log.txt";
		private const string fileOld = "log_old.txt";
		private StreamWriter writer;

		public TraceToFileListener()
		{
			try
			{
				if (File.Exists(file))
				{
					if (File.Exists(fileOld))
						File.Delete(fileOld);
					File.Move(file, fileOld);
				}
			}
			catch { }

			writer = new StreamWriter(file);
			writer.AutoFlush = true;

		}

		#region IDisposable Support
		private bool disposedValue = false;
		protected override void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (writer != null)
						writer.Dispose();
					writer = null;
				}
				disposedValue = true;
			}
			base.Dispose(disposing);
		}
		public new void Dispose()
		{
			Dispose(true);
		}
		#endregion

		public override void Write(string message)
		{
			if (writer != null)
				writer.Write(message);
		}

		public override void WriteLine(string message)
		{
			if (writer != null)
				writer.WriteLine(message);
		}

	}
}
