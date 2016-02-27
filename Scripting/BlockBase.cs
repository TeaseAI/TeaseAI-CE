using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// Foundation for root level blocks, eg a script, or a list.
	/// </summary>
	public class BlockBase : Line
	{
		public enum Validation { NeverRan = 0, Running, Passed, Failed }
		public Validation Valid { get; private set; }

		public Logger Log;
		public readonly GroupInfo Group;


		public BlockBase(int lineNumber, string key, Line[] lines, GroupInfo group, Logger log) : base(lineNumber, key, lines)
		{
			Group = group;
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

	}
}
