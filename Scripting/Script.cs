using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	public class Script : BlockBase
	{
		public Script(string key, Line[] lines, string[] tags, GroupInfo group, Logger log)
			: base(key, lines, tags, group, log)
		{ }
	}
}
