using Compiler.Analyzers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	internal class Program
	{
		static void Main(string[] args)
		{
			using (StreamReader sr = new StreamReader("Pascal_code.txt"))
			{
				using (StreamWriter sw = new StreamWriter("Compiler_Work.txt"))
				{
					Syntaxer syntaxer = new Syntaxer(sw, sr);
					syntaxer.Program();
					sw.Write("\nЗначения переменных:\n"+ Semanter.ToString() + "\nРезультат программы:\n" + Generator.Result);
				}
			}
		}
	}
}
