using System;

namespace TeaseAI_CE.Scripting.VType
{
	public class TimeFrame : IKeyed, IVType
	{
		public TimeSpan Value;
		public TimeFrame(TimeSpan value)
		{
			Value = value;
		}
		public override bool Equals(object obj)
		{
			return Value.Equals(obj);
		}
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public Variable Evaluate(Context sender, Variable left, Operators op, Variable right)
		{
			var log = sender.Root.Log;
			bool validating = sender.Root.Valid == BlockBase.Validation.Running;
			object l = left.Value, r = right.Value;
			switch (op)
			{
				case Operators.Add:
					if (l is TimeFrame && r is TimeFrame)
					{
						try
						{ return new Variable((TimeFrame)l + (TimeFrame)r); }
						catch (OverflowException ex)
						{ log.Error(ex.Message); }
					}
					return null;
				case Operators.Subtract:
					if (l is TimeFrame && r is TimeFrame)
					{
						try
						{ return new Variable((TimeFrame)l - (TimeFrame)r); }
						catch (OverflowException ex)
						{ log.Error(ex.Message); }
					}
					return null;

				case Operators.More:
					if (l is TimeSpan && r is TimeSpan)
						return new Variable((TimeSpan)l > (TimeSpan)r);
					return null;
				case Operators.Less:
					if (l is TimeSpan && r is TimeSpan)
						return new Variable((TimeSpan)l < (TimeSpan)r);
					return null;
			}
			return null;
		}

		public Variable Get(Key key, Logger log = null)
		{
			if (key.AtEnd)
			{
				// ToDo : Error
			}

			switch (key.Next())
			{
				case "totalhours":
					return new Variable((float)Value.TotalHours);
					// ToDo : more like ^^
			}
			// ToDo : Error unknown sub key
			return null;
		}


		public static bool operator >(TimeFrame l, TimeFrame r)
		{
			return l.Value > r.Value;
		}
		public static bool operator <(TimeFrame l, TimeFrame r)
		{
			return l.Value < r.Value;
		}

		public static TimeFrame operator +(TimeFrame l, TimeFrame r)
		{
			return new TimeFrame(l.Value + r.Value);
		}
		public static TimeFrame operator -(TimeFrame l, TimeFrame r)
		{
			return new TimeFrame(l.Value - r.Value);
		}
	}
}
