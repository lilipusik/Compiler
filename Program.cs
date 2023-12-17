using Compiler.Analyzers;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Compiler
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string folders_path	 = @"C:\Users\Юлия\source\repos\Compiler\tests";

			string pascal_folder = folders_path + @"\pascal_codes";
			string outtxt_folder = folders_path + @"\output txts";
			string css_folder	 = folders_path + @"\css";
			string exes_folder	 = folders_path + @"\exes";

			string pascal_path	 = pascal_folder + @"\Pascal_code.txt";
			string output_path	 = outtxt_folder + @"\Compiler_Work.txt";
			string exe_path		 = exes_folder	 + @"\My_compiler.exe";
			string csharp_path	 = css_folder	 + @"\Csharp_code.cs";

			Generator.Create_Folder(new List<string>() { pascal_folder, outtxt_folder, css_folder, exes_folder });

			// LEXICAL AND PARSER
			using (StreamReader sr = new StreamReader(pascal_path))
			{
				using (StreamWriter sw = new StreamWriter(output_path))
				{
					Syntaxer syntaxer = new Syntaxer(sw, sr);
					syntaxer.Program();
				}
			}

			// GENERATED
			try
			{
				if (Syntaxer.Count_Errors > 0) throw new Exception("Errors found. Read output txt file!");

				Generator.Translate_Pascal_To_CSharp();

				Console.WriteLine("\nCreating cs file...");
				File.WriteAllLines(csharp_path, new[] { Generator.CSharp_code });

				Console.WriteLine("\nCompiling...");
				CSharpCodeProvider provider = new CSharpCodeProvider();
				
				CompilerResults results = provider.CompileAssemblyFromSource(new CompilerParameters(new string[0], exe_path) { GenerateExecutable = true }, Generator.CSharp_code);
				if (results.Errors.Count > 0) throw new Exception("Error in the created cs file!");
				
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("\nDone!");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\n" + ex.Message);
			}
			finally
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("\nPress any key");
				Console.ReadKey();
			}
		}
	}
}
