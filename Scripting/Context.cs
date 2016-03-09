using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	public class Context
	{
		public readonly Controller Controller;
		public readonly BlockBase Root;
		public readonly Line Block;

		/// <summary> Current line in Block.Lines </summary>
		public int Line;
		/// <summary> If true, repeat current line and block. </summary>
		public bool Repeat = false;
		/// <summary> Vairables local to this scope. </summary>
		public Dictionary<string, Variable> Variables;
		/// <summary> Results of last if statement, could easily just be a local variable. </summary>
		public bool LastIf = false;
		/// <summary> If true clear stack, and exit this scope. </summary>
		public bool Exit = false;
		/// <summary> If true exit just this scope. </summary>
		public bool Return = false;
		/// <summary> If true stop executing the line. </summary>
		public bool ExitLine = false;

		public Context(Controller controller, BlockBase root, Line block, int line, Dictionary<string, Variable> variables)
		{
			Controller = controller;
			Root = root;
			Block = block;
			Line = line;
			Variables = variables;
		}

		public Variable GetVariable(string key)
		{
			if (key.StartsWith("local"))
			{
				Variable result;
				if (!Variables.TryGetValue(key, out result))
					Variables[key] = result = new Variable();
				return result;
			}
			else
				return Controller.Personality.GetVariable(key, this);
		}
	}
}
