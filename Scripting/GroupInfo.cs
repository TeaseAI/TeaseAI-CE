using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// Info of all scripts contained in a group(file).
	/// </summary>
	public class GroupInfo
	{
		public readonly string Path;
		public readonly string Key;
		public readonly Logger Log;
		public readonly BlockBase[] Blocks;
		public GroupInfo(string path,string key, Logger log, BlockBase[] blocks)
		{
			Path = path;
			Key = key;
			Log = log;
			Blocks = blocks;
		}
	}
}
