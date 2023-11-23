using Compiler.Analyzers;
using Compiler.Tokens;
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
			using(StreamReader sr = new StreamReader("Pascal_code.txt"))
			{
				string line; int line_number = 0;
				while ((line = sr.ReadLine()) != null)
				{
					File_Work file = new File_Work(line);
					Console.WriteLine(line);
					string lexeme = file.Get_Lexeme();
					Position position = new Position(line_number, file.Get_Position().Get_Position().Item2);

					Lexer lexer = new Lexer(lexeme, position);
					Token token = lexer.Get_Token();
					line_number++;
				}
			}
		}
	}
}
