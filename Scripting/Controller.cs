using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MyResources;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// Controllers are the part that figures out what the personlity is going to say next.
	/// </summary>
	public class Controller : IKeyed
	{
		public string Id { get; private set; }

		public readonly Personality Personality;
		public VM VM { get { return Personality.VM; } }

		/// <summary>
		/// Time inbetween updates.
		/// </summary>
		public int Interval;
		internal Stopwatch timmer = new Stopwatch(); // ToDo : Stopwatch is not optimal.

		public delegate void OutputDelegate(Personality p, string text);
		public OutputDelegate OnOutput;

		private Stack<Context> stack = new Stack<Context>();
		private List<Context> queue = new List<Context>();

		public bool AutoFill = false;

		private Variable startQuery = new Variable();
		private Variable emptyQuery = new Variable();

		// ToDo : A shorter list of enabled(or disabled) scripts, based off of the personalities list.

		private StringBuilder output = new StringBuilder();

		internal Controller(Personality personality, string id)
		{
			Personality = personality;
			Id = VM.KeyClean(id);
		}

		public void Tick()
		{
			if (Personality.Enabled == false)
				return;

			while (next(output) && output.Length == 0)
			{ }

			if (OnOutput != null && output.Length > 0)
				OnOutput.Invoke(Personality, output.ToString());
			output.Clear();
		}

		/// <summary>
		/// Executes the next line on the stack.
		/// </summary>
		/// <param name="output"></param>
		/// <returns>false if there was nothing to do.</returns>
		internal bool next(StringBuilder output)
		{
			if (AutoFill && stack.Count == 0 && queue.Count == 0 && emptyQuery.IsSet)
			{
				AddFromEmptyQuery(new Logger("Controller." + Id));
			}

			// populate stack with items in the queue.
			if (queue.Count > 0)
			{
				lock (queue)
				{
					foreach (Context item in queue)
						stack.Push(item);
					queue.Clear();
				}
			}

			if (stack.Count == 0)
				return false;

			// try to execute top item.
			var scope = stack.Peek();
			if (scope.Line >= scope.Block.Lines.Length)
			{
				stack.Pop();
				return next(output);
			}
			else
			{
				scope.Root.Log.SetId(scope.Block.Lines[scope.Line].LineNumber);
				// exec current line
				Line line = scope.Block.Lines[scope.Line];
				Personality.VM.ExecLine(scope, line.Data, output);

				if (scope.ExitLine)
					scope.Repeat = false;

				// always continue when validating.
				if (scope.Root.Valid == BlockBase.Validation.Running)
				{
					scope.Exit = false;
					scope.Return = false;
				}

				// advance to next line, if repeat is false.
				if (scope.Repeat == false)
					++scope.Line;
				else
					scope.Repeat = false;

				if (scope.Exit)
					stack.Clear();
				else if (scope.Return)
					stack.Pop();
				// push sub block if exists
				else if (line.Lines != null && scope.ExitLine == false)
				{
					stack.Push(new Context(this, scope.Root, line, 0, new Dictionary<string, Variable>(scope.Variables)));
					if (stack.Count > 128)
					{
						scope.Root.Log.ErrorF(StringsScripting.Formatted_Stack_too_big, 128);
						stack.Clear();
					}
				}
				// pop off stack, if no lines left.
				else if (scope.Line >= scope.Block.Lines.Length)
					stack.Pop();

				scope.ExitLine = false;
				return true;
			}
		}
		/// <summary>
		/// Enqueue a root block to be added to the stack.
		/// </summary>
		public void Add(BlockBase root)
		{
			if (root == null)
			{
				Logger.Log(null, Logger.Level.Error, StringsScripting.Script_null_add_stack);
				return;
			}
			lock (queue)
			{
				queue.Add(new Context(this, root, root, 0, new Dictionary<string, Variable>()));

				if (queue.Count > 128)
				{
					root.Log.ErrorF(StringsScripting.Formatted_Queue_too_big, 128);
					queue.Clear();
				}
			}
		}
		public bool AddFromStartQuery(Logger log)
		{
			if (startQuery == null || !startQuery.IsSet)
				return false;
			Add(VM.QueryScript(startQuery, log));
			return true;
		}
		public bool AddFromEmptyQuery(Logger log)
		{
			if (emptyQuery == null || !emptyQuery.IsSet)
				return false;
			Add(VM.QueryScript(emptyQuery, log));
			return true;
		}

		public void Input(Personality p, string text)
		{
			// ToDo : Finsh input.
			if (OnOutput != null)
				OnOutput(p, Personality.VM.InputToShorthand(text));
		}

		public void WriteValues(string prefix, StringBuilder sb)
		{
			writeValue(prefix, sb, "startQuery", startQuery);
			writeValue(prefix, sb, "emptyQuery", emptyQuery);
		}
		private void writeValue(string prefix, StringBuilder sb, string key, Variable v)
		{
			if (v == null || !v.CanWriteValue())
				return;
			sb.Append(prefix);
			sb.Append("#(.");
			sb.Append(key);
			sb.Append("=");
			v.WriteValue(sb);
			sb.AppendLine(")");
		}

		#region IKeyed
		public Variable Get(Key key, Logger log = null)
		{
			if (key.AtEnd)
			{
				// ToDo : Error
				return null;
			}
			if (key.NextIf("startquery"))
				return startQuery.Get(key, log);
			if (key.NextIf("emptyquery"))
				return emptyQuery.Get(key, log);
			return Personality.Get(key, log);
		}
		#endregion
	}
}
