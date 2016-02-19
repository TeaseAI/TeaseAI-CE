using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	public class Line
	{
		public readonly int LineNumber;
		public readonly string Data;
		public readonly Line[] Lines;
		public Line(int lineNumber, string data, Line[] lines)
		{
			LineNumber = lineNumber;
			Data = data;
			Lines = lines;
		}
		public override string ToString()
		{
			if (Lines != null && Lines.Length > 0)
				return string.Format("[{0}][block]: '{1}'", LineNumber, Data);
			else
				return string.Format("[{0}]: '{1}'", LineNumber, Data);
		}
	}
}
