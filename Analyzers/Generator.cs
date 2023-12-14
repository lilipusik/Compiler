using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Compiler.Analyzers;

namespace Compiler
{
	class Generator
	{
		private string cur_scarp_code;
		public static string CSharp_Code { get; private set; }

		Dictionary<string, float> variables_float_int = new Dictionary<string, float>();
		Dictionary<string, string> variables_string = new Dictionary<string, string>();

		public Generator() { }

		public void Create_EXE()
		{

		}

		public void Traslate_VAR()
		{
			foreach (var variable in Semanter.Get_Variables())
			{
				string lexeme = variable.Value.ToString();
				string new_lexeme = string.Empty;
				if (lexeme == "FLOAT" || lexeme == "STRING") new_lexeme = lexeme.ToLower();
				else if (lexeme == "INTEGER") new_lexeme = "int";
				else if (lexeme == "BOOLEAN") new_lexeme = "bool";
				cur_scarp_code += new_lexeme + " " + variable.Key + ";\n\t\t\t";    // value - type, key - name
			}
		}

		public void Add_Code(Function_type type, string lexeme)
		{
			switch(type)
			{
				case Function_type.READLN: cur_scarp_code += $"\n\t\t\t{lexeme} = Console.ReadLine();"; break;
				case Function_type.WRITELN: cur_scarp_code += $"\n\t\t\tConsole.WriteLine({lexeme});"; break;
			}
		}

		public void Add_Code(string code)
		{
			cur_scarp_code += code;
		}

		private void Translate_Pascal_To_CSharp()
		{
			CSharp_Code = 
$@"
using System;
	internal class Program
	{{
		static void Main(string[] args)
		{{
			{cur_scarp_code}
		}}
	}}";
		}

		public string Get_CSharp_Code()
		{
			Translate_Pascal_To_CSharp();
			return CSharp_Code;
		}
	}
}
