﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	public class Script : BlockBase
	{
		public Script(int lineNumber, string key, Line[] lines, GroupInfo group, Logger log) : base(lineNumber, key, lines, group, log)
		{ }
	}
}
