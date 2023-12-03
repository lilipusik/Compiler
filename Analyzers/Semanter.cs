using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	internal class Semanter
	{
		private static Dictionary<string, Const_type> variables_type = new Dictionary<string, Const_type>();
		private static List<string> variables_assignment = new List<string>();

		public static void Add_Variables(List<string> names, Const_type type)
		{
			names.ForEach(x => variables_type.Add(x, type));
		}

		public static bool Has_Variable(string name)
		{
			return variables_type.ContainsKey(name);
		}

		public static Const_type Type_Variable(string name)
		{
			return variables_type[name];
		}

		public static void New_Assignment(string name)
		{
			variables_assignment.Add(name);
		}

		public static bool Is_Assignment(string name)
		{
			return variables_assignment.Contains(name);
		}
	}
}
