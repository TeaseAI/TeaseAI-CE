using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MyResources;

namespace TeaseAI_CE.Scripting
{
	/// <summary> Operators that can be used with variables. </summary>
	public enum Operators
	{
		// Math
		Add,
		Subtract,
		Multiply,
		Divide,
		// Logic
		Not,
		Equal,
		More,
		Less,
		And,
		Or,
		//
		Assign,
	}

	/// <summary>
	/// The base variable class, should be used for [bool, float, string]
	/// by-reference thread-safe
	/// </summary>
	public class Variable
	{
		protected object _value = null;
		public object Value { get { return getObj(); } set { setObj(value); } }
		/// <summary> Does variable have a value? </summary>
		public virtual bool IsSet { get { return getObj() != null; } }

		/// <summary> If true scripts can only read. </summary>
		public bool Readonly = false;

		public Variable() { }
		public Variable(string value)
		{ _value = value; }
		public Variable(bool value)
		{ _value = value; }
		public Variable(float value)
		{ _value = value; }
		public Variable(DateTime value)
		{ _value = value; }
		public Variable(TimeSpan value)
		{ _value = value; }

		protected virtual object getObj()
		{
			return _value;
		}
		protected virtual void setObj(object value)
		{
			Interlocked.Exchange(ref _value, value);
		}

		public override string ToString()
		{
			if (IsSet)
				return "Value is: " + Value.ToString();
			return "Value is UnSet";
		}

		public virtual void WriteValueUser(Context sender, StringBuilder output)
		{
			if (!IsSet || sender.Root.Valid == BlockBase.Validation.Running)
				return;

			if (Value is List)
				((List)Value).Execute(sender, output);
			else
				output.Append(Value.ToString());
		}

		/// <summary>
		/// Use to write a value as a string, when writing to files.
		/// </summary>
		/// <param name="sb"></param>
		public virtual void WriteValue(StringBuilder sb)
		{
			// ToDo : Handle other types like scripts.
			if (!IsSet)
				return;
			if (Value is string)
			{
				sb.Append('"');
				var str = (string)Value;
				str = str.Replace("\"", "\\\"").Replace("\n", "\\n");
				sb.Append(str);
				sb.Append('"');
			}
			else if (Value is float || Value is bool)
			{
				sb.Append(Value.ToString());
			}
			else if (Value is DateTime)
			{
				sb.Append("Date(\"");
				sb.Append(((DateTime)Value).ToString("G"));
				sb.Append("\")");
			}
			else if (Value is TimeSpan)
			{
				sb.Append("Time(\"");
				sb.Append(((TimeSpan)Value).ToString("G"));
				sb.Append("\")");
			}
			else if (Value is VariableQuery.Item)
			{
				// ToDo : Save query variables.
			}
			else
				sb.Append("Unsupported_Type");
		}

		public static Variable Evaluate(Context sender, Variable left, Operators op, Variable right)
		{
			var log = sender.Root.Log;
			bool validating = sender.Root.Valid == BlockBase.Validation.Running;

			// logic not
			if (op == Operators.Not)
			{
				if (right == null)
				{
					log.Error(StringsScripting.Evaluate_null_variable);
					return null;
				}
				if (!right.IsSet)
				{
					log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_unset_variable, op.ToString()));
					return null;
				}

				object value = right.Value;
				if (value is bool)
					return new Variable(!(bool)value);
				if (value is VariableQuery.Item)
					return new VariableQuery(VariableQuery.Item.Not((VariableQuery.Item)value));
				if (value is string)
					return new VariableQuery(VariableQuery.Item.Not((string)value));
				log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_type_invalid, op.ToString(), "", value.GetType().Name));
				return null;
			}

			if (left == null || right == null)
			{
				log.Error(StringsScripting.Evaluate_null_variable);
				return null;
			}

			if (left is VariableQuery || right is VariableQuery)
			{
				return VariableQuery.Evaluate(sender, left, op, right);
			}

			object l = left.Value;
			object r = right.Value;

			if (op == Operators.Assign)
			{
				if (!right.IsSet)
				{
					log.Error(StringsScripting.Evaluate_Assign_to_unset);
					return null;
				}

				if (left.Readonly)
				{
					log.Error(StringsScripting.Evaluate_Assign_Readonly);
					return left;
				}

				if (left.IsSet)
				{
					if (l.GetType() == r.GetType())
					{
						if (!validating) // Don't change variable if we are validating.
							left.Value = r;
					}
					else
					{
						log.Error(string.Format(StringsScripting.Formatted_Evaluate_Assign_type_mismatch, l.GetType().Name, r.GetType().Name));
						return null;
					}
				}
				else
				{
					if (validating)
						left.Value = getDefault(right.Value.GetType());
					else
						left.Value = right.Value;
				}
				return left;
			}

			if (!left.IsSet || !right.IsSet)
			{
				log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_unset_variable, op.ToString()));
				return null;
			}

			switch (op)
			{
				// Math
				case Operators.Add:
					if (l is float && r is float)
						return new Variable((float)l + (float)r);
					if (l is string && r is string)
						return new Variable(string.Concat(l, r));
					if ((l is DateTime && r is TimeSpan))
					{
						try
						{ return new Variable((DateTime)l + (TimeSpan)r); }
						catch (ArgumentOutOfRangeException ex)
						{ log.Error(ex.Message); }
					}
					if (l is TimeSpan && r is TimeSpan)
					{
						try
						{ return new Variable((TimeSpan)l + (TimeSpan)r); }
						catch (OverflowException ex)
						{ log.Error(ex.Message); }
					}
					log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_type_invalid, op.ToString(), l.GetType().Name, r.GetType().Name));
					return null;

				case Operators.Subtract:
					if (l is float && r is float)
						return new Variable((float)l - (float)r);
					if (l is string && r is string)
						return new Variable(((string)l).Replace((string)r, ""));
					if ((l is DateTime && r is DateTime))
						return new Variable((DateTime)l - (DateTime)r);
					if ((l is DateTime && r is TimeSpan))
					{
						try
						{ return new Variable((DateTime)l - (TimeSpan)r); }
						catch (ArgumentOutOfRangeException ex)
						{ log.Error(ex.Message); }
					}
					if (l is TimeSpan && r is TimeSpan)
					{
						try
						{ return new Variable((TimeSpan)l - (TimeSpan)r); }
						catch (OverflowException ex)
						{ log.Error(ex.Message); }
					}
					log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_type_invalid, op.ToString(), l.GetType().Name, r.GetType().Name));
					return null;

				case Operators.Multiply:
					if (l is float && r is float)
						return new Variable((float)l * (float)r);
					if (l is float && r is bool)
						return new Variable((float)l * ((bool)r ? 1f : 0f));
					log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_type_invalid, op.ToString(), l.GetType().Name, r.GetType().Name));
					return null;

				case Operators.Divide:
					if (l is float && r is float)
					{
						if (validating)
							return new Variable(default(float));
						float fl = (float)l;
						float fr = (float)r;
						if (fr == 0)
						{
							log.Warning(StringsScripting.Divide_by_zero);
							fr = 1;
						}
						return new Variable(fl / fr);
					}
					log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_type_invalid, op.ToString(), l.GetType().Name, r.GetType().Name));
					return null;

				// Logic
				case Operators.Equal:
					if (l is string && r is string) // for strings we want to ignore the case.
						return new Variable((l as string).Equals((string)r, StringComparison.InvariantCultureIgnoreCase));
					return new Variable(l.Equals(r));
				case Operators.More:
					if (l is float && r is float)
						return new Variable((float)l > (float)r);
					if (l is DateTime && r is DateTime)
						return new Variable((DateTime)l > (DateTime)r);
					if (l is TimeSpan && r is TimeSpan)
						return new Variable((TimeSpan)l > (TimeSpan)r);
					log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_type_invalid, op.ToString(), l.GetType().Name, r.GetType().Name));
					return null;
				case Operators.Less:
					if (l is float && r is float)
						return new Variable((float)l < (float)r);
					if (l is DateTime && r is DateTime)
						return new Variable((DateTime)l < (DateTime)r);
					if (l is TimeSpan && r is TimeSpan)
						return new Variable((TimeSpan)l < (TimeSpan)r);
					log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_type_invalid, op.ToString(), l.GetType().Name, r.GetType().Name));
					return null;
				case Operators.And:
					if (l is bool && r is bool)
						return new Variable((bool)l && (bool)r);
					if (l is string && r is string)
						return VariableQuery.Evaluate(sender, left, op, right);
					log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_type_invalid, op.ToString(), l.GetType().Name, r.GetType().Name));
					return null;
				case Operators.Or:
					if (l is bool && r is bool)
						return new Variable((bool)l || (bool)r);
					if (l is string && r is string)
						return VariableQuery.Evaluate(sender, left, op, right);
					log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_type_invalid, op.ToString(), l.GetType().Name, r.GetType().Name));
					return null;
			}
			log.Error(string.Format(StringsScripting.Formatted_Evaluate_Invalid_operator, op.ToString()));
			return null;
		}
		private static object getDefault(Type type)
		{
			try
			{
				if (type.IsValueType)
					return Activator.CreateInstance(type);
			}
			catch { }
			if (type == typeof(string))
				return "";
			return null;
		}
	}

	/// <summary>
	/// Generic variable, should be used for class typed values.
	/// thread-safe, class T may not be thread-safe.
	/// </summary>
	public class Variable<T> : Variable where T : class
	{
		private new T _value = null;
		public new T Value
		{
			get { return _value; }
			set
			{ Interlocked.Exchange(ref _value, value); }
		}
		public override bool IsSet { get { return _value != null; } }

		public Variable(T value)
		{
			_value = value;
		}

		protected override object getObj()
		{
			return _value;
		}
		protected override void setObj(object value)
		{
			Interlocked.Exchange(ref _value, value as T);
		}
	}

	/// <summary>
	/// Allows one to do nasty things, like using methods as variables.
	/// </summary>
	public class VariableFunc : Variable
	{
		public delegate object GetDelegate();
		public delegate void SetDelegate(object value);
		private GetDelegate get;
		private SetDelegate set;

		/// <param name="getter"></param>
		/// <param name="setter"> if null, Variable is readonly. </param>
		public VariableFunc(GetDelegate getter, SetDelegate setter)
		{
			get = getter;
			set = setter;
			Readonly = set == null;
		}

		public override bool IsSet { get { return set != null || get != null; } }
		protected override object getObj()
		{
			if (get == null)
				return _value = base.getObj();
			else
				return get();
		}
		protected override void setObj(object value)
		{
			_value = value;
			if (set != null)
				set(value);
		}
	}
}
