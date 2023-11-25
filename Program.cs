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

						Position pos = new Position();
						List<Tuple<string, Token>> float_token = new List<Tuple<string, Token>>();
						string string_token = string.Empty;
						do
						{
							// find lexeme and position
							string lexeme = file.Get_Lexeme(position);
							if (lexeme == string.Empty)
							{
								position = file.Get_Position();
								continue;
							}

							// find token
							Lexer lexer = new Lexer(lexeme, position, sw);
							Token token = lexer.Get_Token();

							// maybe token is float
							if (float_token.Count == 0 && token is Constant c && c.Get_Const_Type() == Const_type.INTEGER)
							{
								pos = new Position(position.Get_Position().Item1, position.Get_Position().Item2);
								float_token.Add(new Tuple<string, Token>(lexeme, token));
								continue;
							}
							else if (float_token.Count == 1 && token is KeyWord k && k.Get_Type_KeyWord() == KeyWords.POINT)
							{
								float_token.Add(new Tuple<string, Token>(lexeme, token));
								continue;
							}
							else if (float_token.Count == 2 && token is Constant co && co.Get_Const_Type() == Const_type.INTEGER)
							{
								float_token.Add(new Tuple<string, Token>(lexeme, token));
								lexeme = string.Empty;
								foreach (var tuple in float_token)
									lexeme += tuple.Item1.Replace('.', ',');
								token = new Lexer(lexeme, pos, sw).Get_Token();
							}

							position = new Position(i, file.Get_Position().Get_Position().Item2);
							Console.WriteLine(position);
							sw.WriteLine("lexeme: " + lexeme + " -> " + token + " -> " + position);

						} while (position.Get_Position().Item2 < line.Length);
						i++;
					}
				}
			}
		}
	}
}
