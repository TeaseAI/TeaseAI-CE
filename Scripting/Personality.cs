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

		public string ID
		{
			get { return (string)variables["id"].Value; }
			internal set { variables["id"].Value = value; }
		}
		public string Name
		{
			get { return (string)variables["name"].Value; }
			set { variables["name"].Value = value; }
		}

		private ReaderWriterLockSlim varLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		private Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

		internal Personality(VM vm, string name, string id)
		{
			VM = vm;
			// Variables MUST always be set:
			variables["name"] = new Variable(name);
			variables["id"] = new Variable(id) { Readonly = true };
		}

		public void RunSetup()
		{
			VM.RunSetupOn(this);
		}

		public Variable GetVariable(string key, BlockScope sender)
		{
			// variables starting wtih . is short hand for this personality.
			if (key[0] == '.')
				return getVariable_internal(key.Substring(1, key.Length - 1));
			return VM.GetVariable(key, sender);
		}
		// ToDo : Remove, will not be needed once things are running properly.
		public Variable GetVariable(string key, Logger log)
		{
			// variables starting wtih . is short hand for this personality.
			if (key[0] == '.')
				return getVariable_internal(key.Substring(1, key.Length - 1));

			return VM.GetVariable(key, log);
		}
		internal Variable getVariable_internal(string key)
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