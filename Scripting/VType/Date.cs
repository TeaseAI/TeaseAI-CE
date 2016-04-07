using System;

namespace TeaseAI_CE.Scripting.VType
{
	public class Date : IKeyed, IVType
	{
		public DateTime Value;
		public Date(DateTime value)
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
					if ((l is Date && r is TimeFrame))
					{
						try
						{ return new Variable((Date)l + (TimeFrame)r); }
						catch (ArgumentOutOfRangeException ex)
						{ log.Error(ex.Message); }
					}
					return null;
				case Operators.Subtract:
					if ((l is Date && r is Date))
						return new Variable((Date)l - (Date)r);
					if ((l is Date && r is TimeFrame))
					{
						try
						{ return new Variable((Date)l - (TimeFrame)r); }
						catch (ArgumentOutOfRangeException ex)
						{ log.Error(ex.Message); }
					}
					return null;

				case Operators.More:
					if (l is Date && r is Date)
						return new Variable((Date)l > (Date)r);
					return null;
				case Operators.Less:
					if (l is Date && r is Date)
						return new Variable((Date)l < (Date)r);
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
				case "hour":
					return new Variable((float)Value.Hour);
					// ToDo : more like ^^
			}
			// ToDo : Error unknown sub key
			return null;
		}

		public static bool operator >(Date l, Date r)
		{
			return l.Value > r.Value;
		}
		public static bool operator <(Date l, Date r)
		{
			return l.Value < r.Value;
		}

		public static Date operator +(Date l, TimeFrame r)
		{
			return new Date(l.Value + r.Value);
		}
		public static Date operator -(Date l, TimeFrame r)
		{
			return new Date(l.Value - r.Value);
		}
		public static TimeFrame operator -(Date l, Date r)
		{
			return new TimeFrame(l.Value - r.Value);
		}
	}
}
