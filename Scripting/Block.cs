using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	public class Block
	{
		public readonly Line[] Lines;

		public Block(Line[] lines)
		{
			Lines = lines;
		}

		public override string ToString()
		{
			return Lines.Length + " lines";
		}

		public class Line
		{
			public readonly int LineNumber;
			public readonly string Data;
			public readonly Block SubBlock;
			public Line(int lineNumber, string data, Block subBlock)
			{
				LineNumber = lineNumber;
				Data = data;
				SubBlock = subBlock;
			}
			public override string ToString()
			{
				if (SubBlock != null)
					return string.Format("[{0}][block]: '{1}'", LineNumber, Data);
				else
					return string.Format("[{0}]: '{1}'", LineNumber, Data);
			}
		}
	}
}
