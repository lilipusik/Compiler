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
			// clear older result
			using (StreamWriter writer = new StreamWriter("Compiler_Work.txt"))
			{
				writer.Write("");
			}

			// start program
			using (StreamReader sr = new StreamReader("Pascal_code.txt"))
			{
				string line;
				Position position = new Position();

				while ((line = sr.ReadLine()) != null)
				{
					File_Work file = new File_Work(line);

					using (StreamWriter sw = new StreamWriter("Compiler_Work.txt", true))
					{
						Position pos = new Position();
						List<Tuple<string, Token>> float_token = new List<Tuple<string, Token>>();
						string string_token = string.Empty;
						do
						{
							// find lexeme and position
							string lexeme = file.Get_Lexeme(position);
							position = file.Get_Position();
							if (lexeme == string.Empty) continue;

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

							sw.WriteLine("lexeme: " + lexeme + "\n" + token + "\n");

						} while (position.Get_Position().Item2 < line.Length);
					}
					position = new Position(position.Get_Position().Item1 + 1, 0);
				}
			}
		}
	}
}
