using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// Thread-safe by-reference object to hold values for variables.
	/// </summary>
	public abstract class ValueObj
	{
		public virtual ValueObj Add(ValueObj right) { return null; }
		public virtual ValueObj Subtract(ValueObj right) { return null; }
		public override abstract string ToString();
	}

	public class ValueFunction : ValueObj
	{
		private VM.Function _value;
		public VM.Function Value
		{
			get { return _value; }
			set { Interlocked.Exchange(ref _value, value); }
		}
		public ValueFunction(VM.Function value)
		{ _value = value; }
		public override string ToString()
		{ return Value.ToString(); }
	}
	public class ValueScript : ValueObj
	{
		private Script _value;
		public Script Value
		{
			get { return _value; }
			set { Interlocked.Exchange(ref _value, value); }
		}
		public ValueScript(Script value)
		{ _value = value; }
		public override string ToString()
		{ return Value.ToString(); }
	}
	public class ValuePersonality : ValueObj
	{
		private Personality _value;
		public Personality Value
		{
			get { return _value; }
			set { Interlocked.Exchange(ref _value, value); }
		}
		public ValuePersonality(Personality value)
		{ _value = value; }
		public override string ToString()
		{ return Value.ToString(); }
	}

	public class ValueString : ValueObj
	{
		private string _value;
		public string Value
		{
			get { return _value; }
			set { Interlocked.Exchange(ref _value, value); }
		}
		public ValueString(string value)
		{ _value = value; }

		public override ValueObj Add(ValueObj right)
		{
			if (right is ValueString)
				return new ValueString(Value + ((ValueString)right).Value);
			else if (right is ValueFloat)
				return new ValueString(Value + ((ValueFloat)right).Value);
			return null;
		}

		public override string ToString()
		{ return Value; }
	}

	public class ValueFloat : ValueObj
	{
		private float _value;
		public float Value
		{
			get { return _value; }
			set { Interlocked.Exchange(ref _value, value); }
		}
		public ValueFloat(float value)
		{ _value = value; }

		public override ValueObj Add(ValueObj right)
		{
			if (right is ValueFloat)
				return new ValueFloat(Value + ((ValueFloat)right).Value);
			return null;
		}

		public override ValueObj Subtract(ValueObj right)
		{
			if (right is ValueFloat)
				return new ValueFloat(Value - ((ValueFloat)right).Value);
			return null;
		}

		public override string ToString()
		{ return Value.ToString(); }
	}
}
