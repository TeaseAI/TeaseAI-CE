using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeaseAI_CE.Scripting;
using MyResources;

namespace TeaseAI_CE
{
	public static class Functions
	{
		public static Random Random = new Random();

		public static void AddTo(VM vm)
		{
			vm.AddFunction(restart);

			vm.AddFunction(random);

			vm.AddFunction(wait);


			vm.AddFunction(addContact);
			vm.AddFunction(activateContact);
			vm.AddFunction(removeContact);
		}

		private static Variable random(Context sender, Variable[] args)
		{
			if (args.Length == 0)
				return new Variable((float)Random.NextDouble());

			int min = 0;
			int max = 0;
			if (args.Length == 1)
				max = (int)toNumber(args[0]);
			else if (args.Length == 2)
			{
				min = (int)toNumber(args[0]);
				max = (int)toNumber(args[1]);
			}
			else
				// ToDo : Add to strings:
				sender.Root.Log.WarningF("{0} Only accepts 0-2 arguments!", "Random");

			return new Variable((float)Random.Next(min, max));
		}

		private static Variable wait(Context sender, Variable[] args)
		{
			if (args.Length == 0)
				sender.Root.Log.WarningF(StringsScripting.Formatted_Function_arguments_empty, "Wait");
			else if (args.Length == 1)
				sender.Controller.Add(new Scripting.Events.Timed(new TimeSpan(0, 0, (int)toNumber(args[0])), false));
			else
				sender.Root.Log.WarningF(StringsScripting.Formatted_Function_arguments_more_than_one, "Wait");
			return null;
		}

		private static Variable restart(Context sender, Variable[] args)
		{
			if (args.Length != 0)
				sender.Root.Log.WarningF(StringsScripting.Formatted_Function_arguments_not_empty, "Restart");
			if (sender.Root.Valid == BlockBase.Validation.Passed)
			{
				sender.Repeat = true;
				sender.Line = 0;
			}
			return null;
		}

		private static float toNumber(Variable v)
		{
			float result = 0f;
			if (v != null && v.IsSet)
				float.TryParse(v.Value.ToString(), out result);
			// ToDo 9: Log error if not number.
			return result;
		}


		/// <summary>
		/// Does not actvate but will add to controller.
		/// </summary>
		private static Variable addContact(Context sender, Variable[] args)
		{
			if (args.Length == 0)
				sender.Root.Log.WarningF(StringsScripting.Formatted_Function_arguments_empty, "addContact");
			else
			{
				var p = args[0].Value as Personality;
				if (p != null)
					sender.Controller.Personalities.Add(p);

			}
			return null;
		}
		/// <summary>
		///  Adds contact if not in controller, then actvates it.
		/// </summary>
		private static Variable activateContact(Context sender, Variable[] args)
		{
			if (args.Length == 0)
				sender.Root.Log.WarningF(StringsScripting.Formatted_Function_arguments_empty, "activateContact");
			else
			{
				object v = args[0].Value;
				if (v is Personality)
					sender.Controller.Personalities.Actvate((Personality)v);
				else if (v is float)
					sender.Controller.Personalities.Actvate((int)(float)v);
			}
			return null;
		}
		private static Variable removeContact(Context sender, Variable[] args)
		{
			if (args.Length == 0)
				sender.Root.Log.WarningF(StringsScripting.Formatted_Function_arguments_empty, "removeContact");
			else
			{
				object v = args[0].Value;
				if (v is Personality)
					sender.Controller.Personalities.Remove((Personality)v);
				else if (v is float)
					sender.Controller.Personalities.RemoveAt((int)(float)v);
			}
			return null;
		}
	}
}
