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

		public bool Enabled { get { return EnabledUser; } }

		private Variable enabledUser_var;
		public bool EnabledUser
		{
			get { return (bool)enabledUser_var.Value; }
			set { enabledUser_var.Value = value; }
		}

		private Variable id_var;
		public string ID
		{
			get { return (string)id_var.Value; }
			internal set { id_var.Value = value; }
		}
		private Variable name_var;
		public string Name
		{
			get { return (string)name_var.Value; }
			set { name_var.Value = value; }
		}

		private ReaderWriterLockSlim varLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		private Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

		internal Personality(VM vm, string name, string id)
		{
			VM = vm;
			// Variables MUST always be set:
			variables["name"] = name_var = new Variable(name);
			variables["id"] = id_var = new Variable(id) { Readonly = true };
			variables["enabled_user"] = enabledUser_var = new Variable(true);
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

		public string WriteVariablesToString()
		{
			var sb = new StringBuilder();
			varLock.EnterReadLock();
			try
			{
				sb.Append("Personality.");
				sb.AppendLine(ID);

				foreach (var kvp in variables)
				{
					if (!kvp.Value.IsSet || kvp.Key == "id")
						continue;
					// TAB#(.KEY=VALUE)
					sb.Append("\t#(.");
					sb.Append(kvp.Key);
					sb.Append("=");
					kvp.Value.WriteValue(sb);
					sb.AppendLine(")");
				}
			}
			finally
			{ varLock.ExitReadLock(); }
			return sb.ToString();
		}

		public override string ToString()
		{
			return ID;
		}
	}
}