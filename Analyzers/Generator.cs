using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler
{
	class Generator
	{
		static string cur_csharp_code = string.Empty;

		public static string CSharp_code { get; private set; } = string.Empty;

		// creating folders for saving result
		public static void Create_Folder(List<string> path_directories)	
		{
			path_directories.ForEach(x => Directory.CreateDirectory(x));
		}

		public static void Traslate_VAR()	// declaration of variables
		{
			foreach (var variable in Semanter.Get_Variables())
			{
				string lexeme = variable.Value.ToString();
				string new_lexeme = string.Empty;

				switch (lexeme)
				{
					case "STRING": new_lexeme = lexeme.ToLower(); break;
					case "INTEGER": new_lexeme = "int"; break;
					case "BOOLEAN": new_lexeme = "bool"; break;
					case "FLOAT": new_lexeme = "double"; break;
				}
				cur_csharp_code += new_lexeme + " " + variable.Key + ";\n\t\t\t";    // value - type, key - name
			}
		}

		// input variable value
		public static void Add_Readline(string lexeme, Const_type const_)
		{
			cur_csharp_code += $"\n\t\t\t{lexeme} = ";

			switch (const_)
			{
				case Const_type.STRING: cur_csharp_code += $"Console.ReadLine();\n"; break;
				case Const_type.INTEGER: cur_csharp_code += $"int.Parse(Console.ReadLine());\n"; break;
				case Const_type.FLOAT: cur_csharp_code += $"double.Parse(Console.ReadLine());\n"; break;
			}

			Semanter.New_Assignment(lexeme, "");
		}

		// output variable value
		public static void Add_Writeline(string lexeme)	
		{
			cur_csharp_code += $"\n\t\t\tConsole.WriteLine({lexeme}" + (lexeme.EndsWith(")") ? ";" : ");");
		}

		// assigning a value to a variable
		public static void Add_Expression(string lexeme, string expression)
		{
			expression = expression.Replace(',', '.');
			cur_csharp_code += $"\n\t\t\t{lexeme} = {expression};";
			Semanter.New_Assignment(lexeme, expression);
		}

		// creating cs file text
		public static void Translate_Pascal_To_CSharp()
		{
			CSharp_code =
$@"
using System;
namespace Compiler
{{
	class Program
	{{
		static void Main()
		{{
			{cur_csharp_code}
			Console.ReadKey();
		}}
	}}
}}";
		}
	}
}
