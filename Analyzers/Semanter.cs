using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	internal class Semanter
	{
		private static Dictionary<string, Const_type> variables = new Dictionary<string, Const_type>();

		public static void Add_Variables(List<string> names, Const_type type)
		{
			names.ForEach(x => variables.Add(x, type));
		}

		public static bool Has_Variable(string name)
		{
			return variables.ContainsKey(name);
		}

		public static Const_type Type_Variable(string name)
		{
			return variables[name];
		}
	}
}
