using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// Controllers are the part that figures out what the personlity is going to say next.
	/// </summary>
	public class Controller
	{
		public readonly Personality Personality;

		/// <summary>
		/// Time inbetween updates.
		/// </summary>
		public int Interval;
		internal Stopwatch timmer = new Stopwatch(); // ToDo : Stopwatch is not optimal.

		public delegate void OutputDelegate(Personality p, string text);
		public OutputDelegate OnOutput;

		private Stack<BlockScope> stack = new Stack<BlockScope>();
		private List<BlockScope> queue = new List<BlockScope>();


		// ToDo : A shorter list of enabled(or disabled) scripts, based off of the personalities list.

		private StringBuilder output = new StringBuilder();

		internal Controller(Personality personality)
		{
			Personality = personality;
		}

		public void Tick()
		{
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
			// populate stack with items in the queue.
			if (queue.Count > 0)
			{
				foreach (BlockScope item in queue)
					stack.Push(item);
				queue.Clear();
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
				// ToDo 2: Check for infinte loop.
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
					stack.Push(new BlockScope(this, scope.Root, line, 0, new Dictionary<string, Variable>(scope.Variables)));
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
			// ToDo : queue is not thread safe.
			queue.Add(new BlockScope(this, root, root, 0, new Dictionary<string, Variable>()));
		}
	}
}
