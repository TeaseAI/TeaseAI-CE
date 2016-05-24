using System;
using System.Collections.Concurrent;
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
		private VM vm;
		private ConcurrentDictionary<Tuple<string, string, string>, Variable<T>> blocks = new ConcurrentDictionary<Tuple<string, string, string>, Variable<T>>();

		public bool IsEmpty { get { return blocks.Count == 0; } }

		public BlockGroup(VM vm)
		{
			this.vm = vm;
		}

		public bool TryAdd(string type, string rootKey, string key, T block)
		{
			vm.Dirty = true;
			return blocks.TryAdd(new Tuple<string, string, string>(type, rootKey, key), new Variable<T>(block));
		}
		//public bool TryGet(string key, Variable<T> value)
		//{
		//	return blocks.TryGetValue(key, out value);
		//}

		public Variable Get(Key key, Logger log = null)
		{
			if (key.AtEnd)
			{
				Logger.LogF(log, Logger.Level.Error, StringsScripting.Formatted_IKeyed_Cannot_return_self, key, GetType());
				return null;
			}
			// ToDo : Log error if key remanining is not 3.

			Variable<T> value;
			if (blocks.TryGetValue(new Tuple<string, string, string>(key.Next(), key.Next(), key.Next()), out value))
				return value;

			Logger.LogF(log, Logger.Level.Error, StringsScripting.Formatted_Variable_not_found, key);
			return new Variable<T>(default(T));
		}

		public ICollection<Variable<T>> GetAll()
		{
			return blocks.Values;
		}

	}
}
