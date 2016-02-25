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

		// ToDo : List of enabled(or disabled) scripts

		private string _key;

		private ReaderWriterLockSlim varLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		private Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

		internal Personality(VM vm, string name, string key)
		{
			VM = vm;
			variables["name"] = new Variable(name);
			_key = key;
		}

		public void RunSetup()
		{
			VM.RunSetupOn(this);
		}

		public Variable GetVariable(string key, Logger log)
		{
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
					variables[key] = result = new Variable();
				return result;
			}
			finally
			{ varLock.ExitReadLock(); }
		}
	}
}