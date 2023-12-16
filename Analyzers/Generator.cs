using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler
{
	class Generator
	{
		string lexeme;
		string expression;
		static string cur_csharp_code = string.Empty;

		public static string CSharp_code { get; private set; } = string.Empty;
		public static string Result { get; private set; } = string.Empty;

		public Generator(string lexeme)
		{
			this.lexeme = lexeme;
			expression = Semanter.Get_Value(lexeme);
		}

		public void Expression_Calculator()
		{
			if (expression != string.Empty)
			{
				try
				{
					if (lexeme != expression) cur_csharp_code += $"\n\t\t\t{lexeme} = {expression.Replace(',', '.')};";
					if (lexeme == expression || Semanter.Type_Variable(lexeme) == Const_type.STRING) String_Expression();
					else Math_Expression();
				}
				catch 
				{
					Result = "error";
				}
			}
		}

		private void Math_Expression()
		{
			StringToFormula math_ = new StringToFormula();
			Result += math_.Eval(expression).ToString() + "\n";
		}

		private void String_Expression()
		{
			Result += string.Join("", expression.Split('"', '+')) + "\n";
		}

		public static void Writeline()
		{
			Console.Write(Result);
		}

		public static void Create_Folder(List<string> path_directories)
		{
			path_directories.ForEach(x => Directory.CreateDirectory(x));
		}

		public static void Traslate_VAR()
		{
			foreach (var variable in Semanter.Get_Variables())
			{
				string lexeme = variable.Value.ToString();
				string new_lexeme = string.Empty;
				if (lexeme == "FLOAT" || lexeme == "STRING") new_lexeme = lexeme.ToLower();
				else if (lexeme == "INTEGER") new_lexeme = "int";
				else if (lexeme == "BOOLEAN") new_lexeme = "bool";
				cur_csharp_code += new_lexeme + " " + variable.Key + ";\n\t\t\t";    // value - type, key - name
			}
		}

		public static void Add_Code(Function_type type, string lexeme, Const_type const_)
		{
			switch (type)
			{
				case Function_type.READLN:
					cur_csharp_code += $"\n\t\t\t{lexeme} = ";
					if (const_ == Const_type.STRING) cur_csharp_code += $"Console.ReadLine();\n"; 
					else if (const_ == Const_type.INTEGER) cur_csharp_code += $"int.Parse(Console.ReadLine());\n";
					else if (const_ == Const_type.FLOAT) cur_csharp_code += $"float.Parse(Console.ReadLine());\n";
					break;
				case Function_type.WRITELN: cur_csharp_code += $"\n\t\t\tConsole.WriteLine({lexeme});"; break;
			}
		}

		public static void Translate_Pascal_To_CSharp()
		{
			CSharp_code =
$@"
using System;
namespace Compiler
{{
	class Program
	{{

		void ___run___()
		{{
			Console.WriteLine(""hello"");
		}}
		static void Main()
		{{
			(new Program()).___run___();
			Console.ReadKey();
		}}
	}}
}}";
		}
	}
}
