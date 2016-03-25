using System;
using MyResources;

namespace TeaseAI_CE.Scripting
{
	public static class CoreFunctions
	{
		public static void AddTo(VM vm)
		{
			// if statements
			vm.AddFunction(@if);
			vm.AddFunction(elseif);
			vm.AddFunction(@else);

			vm.AddFunction(not);

			vm.AddFunction(@goto);


			// date time
			vm.AddFunction(date);
			vm.AddFunction(time);

			vm.AddFunction("x", (Context sender, Variable[] args) => { return new Variable(); });

		}

		#region if statements
		private static Variable @if(Context sender, Variable[] args)
		{
			sender.ExitLine = false;
			foreach (var arg in args)
			{
				if (arg.IsSet == false)
					sender.ExitLine = true;
				else if (arg.Value is bool == false)
				{
					sender.Root.Log.ErrorF(StringsScripting.Formatted_Function_invalid_type, "If", arg.Value.GetType().Name, typeof(bool).Name);
					sender.ExitLine = true;
				}
				else if ((bool)arg.Value == false)
					sender.ExitLine = true;
			}

			sender.LastIf = !sender.ExitLine;
			return new Variable(sender.LastIf);
		}
		private static Variable elseif(Context sender, Variable[] args)
		{
			if (sender.LastIf == true)
				return new Variable(false);

			return @if(sender, args);
		}
		private static Variable @else(Context sender, Variable[] args)
		{
			if (args.Length != 0)
				sender.Root.Log.ErrorF(StringsScripting.Formatted_Function_arguments_not_empty, "Else");
			sender.ExitLine = sender.LastIf;
			sender.LastIf = !sender.ExitLine;
			return new Variable(sender.LastIf);
		}
		#endregion

		#region goto, not
		private static Variable not(Context sender, Variable[] args)
		{
			if (args.Length != 0)
				return new Variable(false);
			if (args.Length > 1)
				sender.Root.Log.WarningF(StringsScripting.Formatted_Function_arguments_less_than_two, "Not");
			if (!args[0].IsSet)
				sender.Root.Log.ErrorF(StringsScripting.Formatted_Function_argument_unset, "Not", "");
			else if (args[0].Value is bool)
				return new Variable((bool)args[0].Value);
			else
				sender.Root.Log.ErrorF(StringsScripting.Formatted_Function_invalid_type, "Not", args[0].Value.GetType().Name, typeof(bool).Name);
			return new Variable(true);
		}

		private static Variable @goto(Context sender, Variable[] args)
		{
			if (args.Length == 0)
			{
				sender.Root.Log.ErrorF(StringsScripting.Formatted_Function_arguments_empty, "GoTo");
				return null;
			}
			int i = -1;
			foreach (var arg in args)
			{
				++i;
				if (!arg.IsSet)
					continue;

				BlockBase script = null;

				object value = arg.Value;
				if (value is BlockBase)
					script = (BlockBase)value;
				else if (value is VariableQuery.Item)
					script = sender.Controller.VM.QueryScript((VariableQuery.Item)value, sender.Root.Log);
				else if (value is string) // string can be a single query key.
					script = sender.Controller.VM.QueryScript((string)value, sender.Root.Log);
				else
					sender.Root.Log.ErrorF(StringsScripting.Formatted_Function_invalid_type, "GoTo", arg.Value.GetType().Name, typeof(Script).Name);

				if (script != null)
				{
					if (ReferenceEquals(script, sender.Root))
						sender.Root.Log.Error(StringsScripting.FunctionGoTo_cannot_goto_self);
					else if (sender.Root.Valid != BlockBase.Validation.Running)
						sender.Controller.Add(script);
				}
			}
			return null;
		}
		#endregion

		#region date time
		private static Variable date(Context sender, Variable[] args)
		{
			if (args.Length == 0)
				return new Variable(DateTime.Now);
			if (args.Length == 1)
			{
				if (!args[0].IsSet)
					sender.Root.Log.ErrorF(StringsScripting.Formatted_Function_argument_unset, "Date", "");
				else if (args[0].Value is string)
				{
					DateTime result;
					try
					{
						if (DateTime.TryParse((string)args[0].Value, out result))
							return new Variable(result);
						else
							sender.Root.Log.ErrorF(StringsScripting.Formatted_Unable_to_parse, "Date", (string)args[0].Value, typeof(DateTime).Name);
					}
					catch (ArgumentOutOfRangeException ex)
					{ sender.Root.Log.Error(ex.Message); }
				}
				else
					sender.Root.Log.ErrorF(StringsScripting.Formatted_Function_invalid_type, "Date", args[0].Value.GetType().Name, typeof(string).Name);
				return new Variable(DateTime.Now);
			}
			else
				sender.Root.Log.WarningF(StringsScripting.Formatted_Function_arguments_less_than_two, "Date");
			return new Variable(DateTime.Now);
		}

		private static Variable time(Context sender, Variable[] args)
		{
			if (args.Length == 0)
				return new Variable(TimeSpan.Zero);
			if (args.Length == 1)
			{
				if (!args[0].IsSet)
					sender.Root.Log.ErrorF(StringsScripting.Formatted_Function_argument_unset, "Time", "");
				else if (args[0].Value is string)
				{
					TimeSpan result;
					if (TimeSpan.TryParse((string)args[0].Value, out result))
						return new Variable(result);
					else
						sender.Root.Log.ErrorF(StringsScripting.Formatted_Unable_to_parse, "Time", (string)args[0].Value, typeof(TimeSpan).Name);
				}
				else
					sender.Root.Log.ErrorF(StringsScripting.Formatted_Function_invalid_type, "Time", args[0].Value.GetType().Name, typeof(string).Name);
				return new Variable(TimeSpan.Zero);
			}
			else
				sender.Root.Log.WarningF(StringsScripting.Formatted_Function_arguments_less_than_two, "Time");
			return new Variable(TimeSpan.Zero);
		}
		#endregion

	}
}
