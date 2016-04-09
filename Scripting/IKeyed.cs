using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using MyResources;

namespace TeaseAI_CE.Scripting
{
	public interface IKeyed
	{
		Variable Get(Key key, Logger log = null);
	}

	public class KeyedDictionary<T> : ConcurrentDictionary<string, T>, IKeyed where T : Variable
	{
		private bool newOnEmpty;
		public KeyedDictionary(bool newOnEmpty = true)
		{
			this.newOnEmpty = newOnEmpty;
		}
		public Variable Get(Key key, Logger log = null)
		{
			T result;
			if (!TryGetValue(key.Peek, out result))
			{
				if (newOnEmpty)
					this[key.Peek] = result = Activator.CreateInstance<T>();
				else
				{
					Logger.LogF(log, Logger.Level.Error, StringsScripting.Formatted_Variable_not_found, key);
					return default(T);
				}
			}
			return result.Get(++key, log);
		}
	}
}
