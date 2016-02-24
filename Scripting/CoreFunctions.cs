
namespace TeaseAI_CE.Scripting
{
	public static class CoreFunctions
	{
		public static void AddTo(VM vm)
		{
			#region if statements
			vm.AddFunction("if", (BlockScope sender, Variable[] args) =>
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
			});
			vm.AddFunction("elseif", (BlockScope sender, Variable[] args) =>
			{
				if (sender.LastIf == true)
					return new Variable(false);

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
			});
			vm.AddFunction("else", (BlockScope sender, Variable[] args) =>
			{
				if (args.Length != 0)
					sender.Root.Log.Error("Else stements do not use any agruments!");
				sender.ExitLine = sender.LastIf;
				sender.LastIf = !sender.ExitLine;
				return new Variable(sender.LastIf);
			});
			#endregion
		}
	}
}
