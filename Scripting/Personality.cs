using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// A personality holds anything that defines the personality.
	/// </summary>
	public class Personality
	{
		public readonly VM VM;

		// ToDo : Variables go here
		// ToDo : List of enabled(or disabled) scripts

		private string _key;

		private ReaderWriterLockSlim varLock = new ReaderWriterLockSlim();
		private Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

		internal Personality(VM vm, string name, string key)
		{
			VM = vm;
			variables["name"] = new Variable(name);
			_key = key;
		}

		public Variable GetVariable(string key, Logger log)
		{
			if (key == null || key.Length == 0)
			{
				log.Error("GetVariable: key is empty!");
				return null;
			}

			// variables starting wtih . is short hand for this personality.
			if (key[0] == '.')
				return getVariable_internal(key.Substring(1, key.Length - 1), log);

			return VM.GetVariable(key, log);
		}
		internal Variable getVariable_internal(string key, Logger log)
		{
			varLock.EnterReadLock();
			try
			{
				Variable result;
				if (!variables.TryGetValue(key, out result))
				{
					result = new Variable();
					variables[key] = result;
				}
				return result;
			}
			finally
			{ varLock.ExitReadLock(); }
		}
	}
}