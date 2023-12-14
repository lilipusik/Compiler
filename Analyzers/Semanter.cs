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
		private static Dictionary<string, string> variables_value = new Dictionary<string, string>();

		public static Dictionary<string, Const_type> Get_Variables()
		{
			return variables_type;
		}

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

		public static void New_Assignment<T>(string name, T value)
		{
			variables_value.Add(name, value.ToString());
		}

		public static bool Is_Assignment(string name)
		{
			return variables_value.ContainsKey(name);
		}
	}
}
