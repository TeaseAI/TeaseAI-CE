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

		// ToDo : Replace with proper script system.
		internal List<string> script = new List<string>();

		public VM()
		{
			thread = new Thread(new ThreadStart(threadTick));
		}

		// ToDo : REMOVE/Replace
		public bool Load()
		{
			// This is not proper, it is just to show how the UI/class structure could work.
			try
			{
				using (var sr = new StreamReader(Path.Combine("scripts", "dummy.txt")))
					while (!sr.EndOfStream)
						script.Add(sr.ReadLine());
			}
			catch
			{ return false; }
			return script.Count > 0;
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
