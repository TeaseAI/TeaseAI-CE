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
		private Dictionary<string, ValueObj> variables = new Dictionary<string, ValueObj>();

		internal Personality(VM vm, string name, string key)
		{
			VM = vm;
			variables["name"] = new ValueString(name);
			_key = key;
		}

		public ValueObj GetVariable(string key, Logger log)
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
		internal ValueObj getVariable_internal(string key, Logger log)
		{
			varLock.EnterReadLock();
			try
			{
				ValueObj result;
				if (!variables.TryGetValue(key, out result))
					log.Error("Variable does not exist: " + key);
				return result;
			}
			finally
			{ varLock.ExitReadLock(); }
		}
		public void SetVariable(string key, ValueObj value)
		{
			varLock.EnterWriteLock();
			try
			{
				variables[key] = value;
			}
			finally
			{ varLock.ExitWriteLock(); }
		}
	}
}