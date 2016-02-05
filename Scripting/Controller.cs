using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// Controllers are the part that figures out what the personlity is going to say next.
	/// </summary>
	public class Controller
	{
		private readonly VM vm;
		private readonly Personality personality;

		/// <summary>
		/// Time inbetween updates.
		/// </summary>
		public int Interval;
		internal Stopwatch timmer = new Stopwatch(); // ToDo : Stopwatch is not optimal.

		public delegate void OutputDelegate(string text);
		public OutputDelegate OnOutput;

		// ToDo : Stack goes here. (handles the block local variables, where we are in the script, etc..)
		// ToDo : A shorter list of enabled(or disabled) scripts, based off of the personalities list.


		//Replace with stack:
		private int line = 0;

		internal Controller(VM vm, Personality personality)
		{
			this.vm = vm;
			this.personality = personality;
		}

		public void Tick()
		{
			if (line >= vm.script.Count)
				return;

			if (OnOutput != null)
				OnOutput.Invoke(vm.script[line]);

			++line;
		}
	}
}
