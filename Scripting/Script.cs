using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyResources;

namespace TeaseAI_CE.Scripting
{
	public class Script : BlockBase
	{
		private static Random random = new Random();
		public readonly bool List;

		private VM vm;
		public Script(VM vm, bool isList, string key, Line[] lines, string[] tags, GroupInfo group, Logger log)
			: base(key, lines, tags, group, log)
		{
			this.vm = vm;
			List = isList;
		}

		public void ExecuteAsList(Context sender, StringBuilder output)
		{
			if (Lines.Length == 0)
			{
				sender.Root.Log.Error(StringsScripting.List_empty);
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
			sender.Root.Log.Error(StringsScripting.List_no_return);
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
							sender.Root.Log.Error(string.Format(StringsScripting.Formatted_List_invalid_weight_type, val.GetType().Name));
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
