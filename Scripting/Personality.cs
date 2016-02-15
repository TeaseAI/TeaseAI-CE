using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		// Name is temp, will be a variable.
		private string _name, _key;
		public string Name { get { return _name; } }

		internal Personality(VM vm, string name, string key)
		{
			VM = vm;
			_name = name;
			_key = key;
		}

	}
}