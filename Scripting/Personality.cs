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
	public class Personality : IKeyed
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

		private KeyedDictionary<Variable> variables = new KeyedDictionary<Variable>();

		internal Personality(VM vm, string name, string id)
		{
			VM = vm;
			// Variables MUST always be set:
			variables["name"] = name_var = new Variable(name);
			variables["id"] = id_var = new Variable(id) { Readonly = true };
			variables["enabled_user"] = enabledUser_var = new Variable(true);
			variables["birthday"] = new Variable(new VType.Date(DateTime.MinValue));

			// readonly age, returns DateTime.Now - BirthDay
			variables["age"] = new VariableFunc(() =>
			{
				try
				{
					var span = DateTime.Now - ((VType.Date)variables["birthday"].Value).Value;
					return (float)(new DateTime(1, 1, 1) + span).Year - 1f;
				}
				catch (ArgumentOutOfRangeException)
				{
					return -1f;
				}
			}, null);
		}

		public void RunSetup()
		{
			VM.RunSetupOn(this);
		}

		#region IKeyed
		public Variable Get(Key key, Logger log = null)
		{
			if (key.AtEnd)
				return VM.Get(new Key(log, "personality", (string)id_var.Value));
			if (key.NextIf("self"))
			{
				if (key.AtEnd)
					return VM.Get(new Key(log, "personality", (string)id_var.Value));
				return variables.Get(key, log);
			}
			return VM.Get(key, log);
		}
		#endregion

		public string WriteVariablesToString()
		{
			var sb = new StringBuilder();
			{
				sb.Append("Personality.");
				sb.AppendLine(ID);

				foreach (var kvp in variables)
				{
					if (!kvp.Value.CanWriteValue() || kvp.Key.StartsWith("tmp"))
						continue;
					// TAB#(.KEY=VALUE)
					sb.Append("\t#(.");
					sb.Append(kvp.Key);
					sb.Append("=");
					kvp.Value.WriteValue(sb);
					sb.AppendLine(")");
				}
			}
			return sb.ToString();
		}

		public override string ToString()
		{
			return ID;
		}

	}
}