using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MyResources;
using TeaseAI_CE.Scripting.Events;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// Controllers are the part that figures out what the personality is going to say next.
	/// </summary>
	public class Controller : IKeyed
	{
		public string Id { get; private set; }

		public Contacts Personalities;
		public readonly VM VM;

		/// <summary>
		/// Time in between ticks.
		/// </summary>
		public int Interval;
		private Stopwatch timmer = new Stopwatch(); // ToDo : Stopwatch is not optimal.

		public Action<Personality, string> OnOutput;

		private Stack<Context> stack = new Stack<Context>();
		private List<Context> queue = new List<Context>();

		public bool AutoFill = false;

		private Variable startQuery = new Variable();
		private Variable emptyQuery = new Variable();

		// ToDo : A shorter list of enabled(or disabled) scripts, based off of the personalities list.

		private Queue<IEvent> events = new Queue<IEvent>();

		private StringBuilder output = new StringBuilder();

		internal Controller(VM vm, string id)
		{
			VM = vm;
			Id = VM.KeyClean(id);
			Personalities = new Contacts(VM);
		}

		public bool Contains(Personality p)
		{
			return Personalities.Contains(p);
		}


		internal void stop()
		{
			timmer.Stop();
		}

		internal void tick_internal()
		{
			if (!timmer.IsRunning)
				timmer.Start();
			if (timmer.ElapsedMilliseconds > Interval)
			{
				timmer.Stop();
				timmer.Reset();
				timmer.Start();
				Tick();
			}
		}

		public void Tick()
		{
			// Wait for all events to finish.
			while (events.Count > 0)
			{
				if (!events.Peek().Finished())
					return;
				events.Dequeue();
			}

			Personality personality;
			while (next(out personality, output) && output.Length == 0)
			{ }

			if (output.Length == 0)
				return;
			events.Enqueue(new Timed(new TimeSpan(0, 0, 0, 0, output.Length * 80), false, () =>
			{
				OnOutput?.Invoke(personality, output.ToString());
				output.Clear();
			}));
		}

		/// <summary>
		/// Executes the next line on the stack.
		/// </summary>
		/// <param name="output"></param>
		/// <returns>false if there was nothing to do.</returns>
		internal bool next(out Personality personality, StringBuilder output)
		{
			personality = Personalities.GetActive();
			if (personality == null)
				return false;

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
				return next(out personality, output);
			}
			else
			{
				scope.Root.Log.SetId(scope.Block.Lines[scope.Line].LineNumber);
				// exec current line
				Line line = scope.Block.Lines[scope.Line];
				VM.ExecLine(scope, line.Data, output);

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

		public void Add(IEvent e)
		{
			// ToDo : Not thread safe
			events.Enqueue(e);
		}

		public void Input(Personality p, string text)
		{
			var shorthand = VM.InputToShorthand(text);

			// ToDo 8: If Debugging
			//OnOutput?.Invoke(p, shorthand);
			// else
			OnOutput?.Invoke(p, text);

			var script = VM.GetScriptResponse(shorthand);
			if (script != null)
				Add(script);
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


			var p = Personalities.GetActive();
			if (key.NextIf("contact"))
			{
				int i;
				if (!int.TryParse(key.Next(), out i))
					// ToDo : Error
					return null;
				p = Personalities.Get(i);
			}

			if (p == null)
				return null;
			return p.Get(key, log);
		}
		#endregion
	}
}
