using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MyResources;

namespace TeaseAI_CE.Scripting.VType
{
	public class Query : IVType
	{
		public Operators Operator;
		public bool Notted = false;
		public string Key = null;
		public Query[] Items = null;
		public bool IsOperator { get { return Key == null && Items == null; } }

		private Query() { }
		/// <summary> New item from keys </summary>
		public Query(string lKey, Operators op, string rKey)
		{
			Items = new Query[] { lKey, op, rKey };
		}
		/// <summary> Item on the left gets keys on same array. </summary>
		public Query(Query lItem, Operators op, string rKey)
		{
			Items = new Query[lItem.Items.Length + 2];
			lItem.Items.CopyTo(Items, 0);
			Items[Items.Length - 2] = op;
			Items[Items.Length - 1] = rKey;
		}
		/// <summary> Item on the right get added as a sub item. </summary>
		public Query(string lKey, Operators op, Query rItem)
		{
			Items = new Query[] { lKey, op, rItem };
		}
		/// <summary> Item on the right get added as a sub item. </summary>
		public Query(Query lItem, Operators op, Query rItem)
		{
			Items = new Query[] { lItem, op, rItem };
		}

		public override string ToString()
		{
			if (Items != null)
			{
				string result = "( ";
				if (Notted)
					result = "not( ";
				foreach (Query i in Items)
					result += i.ToString() + " ";
				return result + ")";
			}
			if (Key != null)
			{
				if (Notted)
					return "not \"" + Key + '"';
				return '"' + Key + '"';
			}
			return Operator.ToString();
		}

		public static Query Not(Query item)
		{
			return new Query() { Key = item.Key, Items = item.Items, Notted = true };
		}

		public static implicit operator Query(string key)
		{
			return new Query() { Key = key };
		}
		public static implicit operator Query(Operators op)
		{
			return new Query() { Operator = op };
		}


		/// <summary> Runs query on list and removes if not valid. </summary>
		/// <remarks>Note: This is not effecent at all. </remarks>
		/// <param name="list">Removes items if not in query.</param>
		/// <param name="item">Should be VariableQuery.Value</param>
		public static void QueryReduceByTag(List<Variable<Script>> list, Query item, Logger log)
		{
			int i = 0;
			BlockBase script;
			while (i < list.Count)
			{
				script = list[i].Value;
				if (script == null || !script.HasTags)
					list.RemoveAt(i);
				else if (!(pass(script, item, log) ^ item.Notted))
					list.RemoveAt(i);
				else
					++i;
			}
		}
		/// <summary> Recursive tests test against item. </summary>
		private static bool pass<T>(T test, Query item, Logger log) where T : BlockBase
		{
			if (item.IsOperator)
			{
				log.ErrorF(StringsScripting.Formatted_Unexpected_Operator, item.Operator.ToString());
				return false;
			}
			if (item.Key != null)
			{
				return test.ContainsTag(VM.KeyClean(item.Key, log));
			}

			var items = item.Items;
			if (items.Length == 0)
				return true;
			if (items.Length == 1)
				return pass(test, items[0], log) ^ items[0].Notted;

			int i = 0;
			while (i + 2 < items.Length)
			{
				Query l = items[i++];
				Query o = items[i++];
				Query r = items[i++];
				if (l.IsOperator || r.IsOperator)
				{
					if (l.IsOperator)
						log.ErrorF(StringsScripting.Formatted_Unexpected_Operator, l.Operator);
					if (r.IsOperator)
						log.ErrorF(StringsScripting.Formatted_Unexpected_Operator, r.Operator);
					return false;
				}
				if (!o.IsOperator)
				{
					log.Error(StringsScripting.Expected_operator_got_variable);
					return false;
				}

				bool lResult = pass(test, l, log) ^ l.Notted;
				if (o.Operator == Operators.Or && lResult)
					continue;
				bool rResult = pass(test, r, log) ^ r.Notted;

				if (o.Operator == Operators.Or)
				{
					if (!lResult && !rResult)
						return false;
				}
				else if (o.Operator == Operators.And)
				{
					if (!lResult || !rResult)
						return false;
				}
				else
				{
					log.ErrorF(StringsScripting.Formatted_Unexpected_Operator, o.Operator);
					return false;
				}
			}
			return true;
		}


		public Variable Evaluate(Context sender, Variable left, Operators op, Variable right)
		{
			var log = sender.Root.Log;
			bool validating = sender.Root.Valid == BlockBase.Validation.Running;
			object l = left.Value, r = right.Value;
			switch (op)
			{
				case Operators.Assign:
					if (left.IsSet && false == (l is Query || l is string))
						return null;

					if (r is Query)
						left.Value = (Query)r;
					else if (r is string)
						left.Value = (string)r;
					else
						return null;
					return left;

				case Operators.Not:
					if (r is Query)
						return new Variable(Not((Query)r));
					return null;

				case Operators.And:
				case Operators.Or:
					if (l is Query && r is Query)
						return new Variable(new Query((Query)l, op, (Query)r));
					else if (l is string && r is Query)
						return new Variable(new Query((string)l, op, (Query)r));
					else if (l is Query && r is string)
						return new Variable(new Query((Query)l, op, (string)r));
					else if (l is string && r is string)
						return new Variable(new Query((string)l, op, (string)r));
					return null;
			}
			return null;
		}
	}
}
