using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MyResources;

namespace TeaseAI_CE.Scripting
{
	// ToDo : Change other variable types to a interface for value insted?
	public class VariableQuery : Variable
	{
		private new Item _value;

		public VariableQuery(Item value)
		{
			_value = value;
		}
		protected override object getObj()
		{ return _value; }
		protected override void setObj(object value)
		{ Interlocked.Exchange(ref _value, value as Item); }


		/// <summary> Runs query on list and removes if not valid. </summary>
		/// <remarks>Note: This is not effecent at all. </remarks>
		/// <param name="list">Removes items if not in query.</param>
		/// <param name="item">Should be VariableQuery.Value</param>
		public static void QueryReduceByTag(List<Variable<Script>> list, Item item, Logger log)
		{
			int i = 0;
			while (i < list.Count)
			{
				if (!list[i].IsSet || !pass(list[i].Value, item, log))
					list.RemoveAt(i);
				else
					++i;
			}
		}
		/// <summary> Recursive tests test against item. </summary>
		private static bool pass<T>(T test, Item item, Logger log) where T : BlockBase
		{
			if (item.IsOperator)
			{
				log.Error("");
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
				return pass(test, items[0], log);

			int i = 0;
			while (i + 2 < items.Length)
			{
				Item l = items[i++];
				Item o = items[i++];
				Item r = items[i++];
				if (l.IsOperator || r.IsOperator)
				{
					log.Error("");
					return false;
				}
				if (!o.IsOperator)
				{
					log.Error("");
					return false;
				}

				bool lResult = pass(test, l, log);
				if (o.Operator == Operators.Or && lResult)
					continue;
				bool rResult = pass(test, r, log);

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
					log.Error("");
					return false;
				}
			}
			return true;
		}


		public static new VariableQuery Evaluate(Context sender, Variable left, Operators op, Variable right)
		{
			var log = sender.Root.Log;
			bool validating = sender.Root.Valid == BlockBase.Validation.Running;
			if (left == null || right == null)
			{
				log.Error(StringsScripting.Evaluate_null_variable);
				return null;
			}

			object l = left.Value;
			object r = right.Value;

			if (op == Operators.Assign)
			{
				if (left.Readonly)
				{
					log.Error(StringsScripting.Evaluate_Assign_Readonly);
					return left as VariableQuery;
				}

				Item item = null;
				if (r is Item)
					item = (Item)r;
				else if (r is string)
					item = (string)r;
				else
				{
					log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_type_invalid, op.ToString(), l.GetType().Name, r.GetType().Name));
					return null;
				}

				left.Value = item;
				return left as VariableQuery;
			}


			switch (op)
			{
				case Operators.And:
				case Operators.Or:
					if (l is Item && r is Item)
						return new VariableQuery(new Item((Item)l, op, (Item)r));
					if (l is string && r is Item)
						return new VariableQuery(new Item((string)l, op, (Item)r));
					if (l is Item && r is string)
						return new VariableQuery(new Item((Item)l, op, (string)r));
					if (l is string && r is string)
						return new VariableQuery(new Item((string)l, op, (string)r));
					log.Error(string.Format(StringsScripting.Formatted_Evaluate_Operator_type_invalid, op.ToString(), l.GetType().Name, r.GetType().Name));
					return null;
			}
			log.Error(string.Format(StringsScripting.Formatted_Evaluate_Invalid_operator, op.ToString()));
			return null;
		}

		public class Item
		{
			public Operators Operator;
			public string Key = null;
			public Item[] Items = null;
			public bool IsOperator { get { return Key == null && Items == null; } }

			private Item() { }
			/// <summary> New item from keys </summary>
			public Item(string lKey, Operators op, string rKey)
			{
				Items = new Item[] { lKey, op, rKey };
			}
			/// <summary> Item on the left gets keys on same array. </summary>
			public Item(Item lItem, Operators op, string rKey)
			{
				Items = new Item[lItem.Items.Length + 2];
				lItem.Items.CopyTo(Items, 0);
				Items[Items.Length - 2] = op;
				Items[Items.Length - 1] = rKey;
			}
			/// <summary> Item on the right get added as a sub item. </summary>
			public Item(string lKey, Operators op, Item rItem)
			{
				Items = new Item[] { lKey, op, rItem };
			}
			/// <summary> Item on the right get added as a sub item. </summary>
			public Item(Item lItem, Operators op, Item rItem)
			{
				Items = new Item[] { lItem, op, rItem };
			}

			public static implicit operator Item(string key)
			{
				return new Item() { Key = key };
			}
			public static implicit operator Item(Operators op)
			{
				return new Item() { Operator = op };
			}
		}
	}
}
