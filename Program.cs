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
			using (StreamReader sr = new StreamReader("Pascal_code.txt")) // reading pascal code
			{
				string line; int i = 0;
				Position position = new Position();

				using (StreamWriter sw = new StreamWriter("Compiler_Work.txt")) // writing result compiler work
				{
					while ((line = sr.ReadLine()) != null)
					{
						File_Work file = new File_Work(line, position);
						position = new Position(i, file.Get_Position().Get_Position().Item2);

						Position pos = new Position();
						List<Tuple<string, Token>> float_token = new List<Tuple<string, Token>>();
						string string_token = string.Empty;
						do
						{
							// find lexeme and position
							string lexeme = file.Get_Lexeme(position);
							if (lexeme == string.Empty)
							{
								position = new Position(i, file.Get_Position().Get_Position().Item2);
								continue;
							}

							// find token
							Lexer lexer = new Lexer(lexeme, position, sw);
							Token token = lexer.Get_Token();

							position = new Position(i, file.Get_Position().Get_Position().Item2);
							sw.WriteLine("lexeme: " + lexeme + "\n" + token + " -> " + position + "\n");

						} while (position.Get_Position().Item2 < line.Length);
						i++;
					}
				}
			}
		}
	}
}
