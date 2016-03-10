using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	public class List : BlockBase
	{
		private static Random random = new Random();

		private VM vm;
		public List(VM vm, int lineNumber, string key, Line[] lines, GroupInfo group, Logger log) : base(lineNumber, key, lines, group, log)
		{
			this.vm = vm;
		}

		public void Execute(Context sender, StringBuilder output)
		{
			if (Lines.Length == 0)
			{
				sender.Root.Log.Error("List has zero items!");
				return;
			}
			if (Lines.Length == 1)
			{
				vm.ExecLine(sender, Lines[0].Data, output);
				return;
			}

			// get the weight of each line.
			var weight = getWeight(sender);
			float max = weight[weight.Length - 1];

			// get a random value from 0 to max.
			float r = (float)(random.NextDouble() * max);
			// find the weight that is within r.
			float w;
			for (int i = 0; i < weight.Length; ++i)
			{
				w = weight[i];
				if (w <= 0f)
					continue;
				else if (r <= w)
				{
					vm.ExecLine(sender, Lines[i].Data, output);
					return;
				}
			}
			sender.Root.Log.Error("List.Execute did not return a value!");
		}
		private float[] getWeight(Context sender)
		{
			var weight = new float[Lines.Length];
			float max = 0f;

			for (int l = 0; l < Lines.Length; ++l)
			{
				Line line = Lines[l];
				if (line.Data.StartsWith("#x(", StringComparison.InvariantCultureIgnoreCase))
				{
					int i = 1;
					string key;
					Variable[] args;
					vm.execSplitCommand(sender, line.Data, ref i, out key, out args);
					if (args.Length > 0 && args[0].IsSet)
					{
						object val = args[0].Value;
						if (val is float)
							weight[l] = max += (float)val;
						else if (val is bool)
						{
							if ((bool)val == true)
								weight[l] = max += 1f;
							else
								weight[l] = -1f;
						}
						else
						{
							sender.Root.Log.Error(string.Format("Expecting float or bool but got {0}", val.GetType().Name));
							weight[l] = -1f;
						}
					}
				}
				else
					weight[l] = max += 1f;
			}
			return weight;
		}
	}
}
