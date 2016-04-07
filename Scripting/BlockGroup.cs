using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// 
	/// </summary>
	public class BlockGroup<T> : IKeyed where T : BlockBase, IKeyed
	{
		private KeyedDictionary<Variable<T>> blocks = new KeyedDictionary<Variable<T>>(false);

		public Variable Get(Key key, Logger log = null)
		{
			if (key.AtEnd)
			{
				// ToDo : Error
				return null;
			}
			return blocks.Get(key, log);
		}
	}
}
