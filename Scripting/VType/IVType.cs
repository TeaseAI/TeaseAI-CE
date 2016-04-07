using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting.VType
{
	public interface IVType
	{
		Variable Evaluate(Context sender, Variable left, Operators op, Variable right);
	}
}
