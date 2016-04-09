using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyResources;

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
				Logger.LogF(log, Logger.Level.Error, StringsScripting.Formatted_IKeyed_Cannot_return_self, key, GetType());
				return null;
			}
			return blocks.Get(key, log);
		}
	}
}
