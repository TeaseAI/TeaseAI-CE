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

	}
}
