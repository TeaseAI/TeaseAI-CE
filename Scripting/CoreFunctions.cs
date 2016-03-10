using System;

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
					sender.Root.Log.Error(string.Format("Function if, expected arg type bool, but got '{0}'", arg.Value.GetType().Name));
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
				sender.Root.Log.Error("Else stements do not use any agruments!");
			sender.ExitLine = sender.LastIf;
			sender.LastIf = !sender.ExitLine;
			return new Variable(sender.LastIf);
		}
		#endregion

		#region not goto
		private static Variable not(Context sender, Variable[] args)
		{
			if (args.Length != 0)
				return new Variable(false);
			if (args.Length > 1)
				sender.Root.Log.Warning("Function not, only supports one or zero argument(s)!");
			if (!args[0].IsSet)
				sender.Root.Log.Error("Function not, argument is not set!");
			else if (args[0].Value is bool)
				return new Variable((bool)args[0].Value);
			else
				sender.Root.Log.Error(string.Format("Function not, Invalid arg type {0}", args[0].Value.GetType().Name));
			return new Variable(true);
		}

		private static Variable @goto(Context sender, Variable[] args)
		{
			if (args.Length == 0)
			{
				sender.Root.Log.Error("GoTo requires args.");
				return null;
			}
			int i = -1;
			foreach (var arg in args)
			{
				++i;
				if (!arg.IsSet)
					continue;

				if (arg.Value is Script)
				{
					sender.Controller.Add((Script)arg.Value);
				}
				else
					sender.Root.Log.Error(string.Format("Arg {0} is of unsupported type '{1}'.", i, arg.Value.GetType().Name));
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
					sender.Root.Log.Error("Arg 0 is UnSet!");
				else if (args[0].Value is string)
				{
					DateTime result;
					try
					{
						if (DateTime.TryParse((string)args[0].Value, out result))
							return new Variable(result);
					}
					catch (ArgumentOutOfRangeException ex)
					{ sender.Root.Log.Error(ex.Message); }
				}
				else
					sender.Root.Log.Error(string.Format("Arg 0 is of unsupported type '{0}'.", args[0].Value.GetType().Name));
				return new Variable(DateTime.Now);
			}
			else
			{
				for (int i = 0; i < args.Length; ++i)
				{
					if (!args[i].IsSet)
					{
						sender.Root.Log.Error(string.Format("Arg {0} is UnSet!", i));
						continue;
					}
					var value = args[i].Value;
					if (value is string)
					{
						DateTime result;
						try
						{
							if (DateTime.TryParse((string)args[0].Value, out result))
								return new Variable(result);
						}
						catch (ArgumentOutOfRangeException ex)
						{ sender.Root.Log.Error(ex.Message); }
					}
					else
						sender.Root.Log.Error(string.Format("Arg 0 is of unsupported type '{0}'.", args[0].Value.GetType().Name));
					return new Variable(DateTime.Now);
				}
			}
			return new Variable(DateTime.Now);
		}

		private static Variable time(Context sender, Variable[] args)
		{
			if (args.Length == 0)
				return new Variable(TimeSpan.Zero);
			if (args.Length == 1)
			{
				if (!args[0].IsSet)
					sender.Root.Log.Error("Arg 0 is UnSet!");
				else if (args[0].Value is string)
				{
					TimeSpan result;
					if (TimeSpan.TryParse((string)args[0].Value, out result))
						return new Variable(result);
				}
				else
					sender.Root.Log.Error(string.Format("Arg 0 is of unsupported type '{0}'.", args[0].Value.GetType().Name));
				return new Variable(TimeSpan.Zero);
			}
			else
			{
				for (int i = 0; i < args.Length; ++i)
				{
					if (!args[i].IsSet)
					{
						sender.Root.Log.Error(string.Format("Arg {0} is UnSet!", i));
						continue;
					}
					var value = args[i].Value;
					if (value is string)
					{
						TimeSpan result;
						if (TimeSpan.TryParse((string)args[0].Value, out result))
							return new Variable(result);
					}
					else
						sender.Root.Log.Error(string.Format("Arg 0 is of unsupported type '{0}'.", args[0].Value.GetType().Name));
					return new Variable(TimeSpan.Zero);
				}
			}
			return new Variable(TimeSpan.Zero);
		}
		#endregion
	}
}
