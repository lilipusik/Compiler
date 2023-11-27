using Compiler.Analyzers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	enum Blocks { PROGRAM, PROGRAM_NAME, SEMICOLON, VAR, BLOCK, POINT };

	class Sytaxer
	{
		StreamReader reader; StreamWriter writer;

		string line;
		Position position;
		string lexeme;
		Token cur_token;
		List<KeyWords> startes = new List<KeyWords>();

		public Sytaxer(StreamReader sr, StreamWriter sw) 
		{
			this.reader = sr;
			this.writer = sw;
			position = new Position();
			line = sr.ReadLine();
		}

		private void Print_Error(string message, string type)
		{
			writer.WriteLine(new Error(message, position, lexeme, type));
		}

		public void Next_Token()
		{

		}

		public void Program(Blocks order)
		{
			Next_Token();
			bool flag = true;
			while (flag)
			{
				switch (order)
				{
					case Blocks.PROGRAM:
						if (Accept(KeyWords.PROGRAM))
						{
							order = Blocks.PROGRAM_NAME;
							Next_Token();
						}
						else
						{
							Print_Error("Uncorrect program start", "Syntax error");
							flag = false;
						}
						break;

					case Blocks.PROGRAM_NAME:
						if (Accept(Token_type.IDENTIFIER))
						{
							order = Blocks.SEMICOLON;
							Next_Token();
						}
						else
						{
							Print_Error("Uncorrect program name", "Syntax error");
							flag = false;
						}
						break;

					case Blocks.SEMICOLON:
						if (Accept(KeyWords.SEMICOLON))
						{
							order = Blocks.VAR;
							Next_Token();
						}
						else
						{
							Print_Error("Not found semicolon after program name", "Syntax error");
							flag = false;
						}
						break;

					case Blocks.VAR: break;

					case Blocks.BLOCK: break;

					case Blocks.POINT: break;
				}
			}
		}

		private bool Accept(Token_type token_type)
		{
			return cur_token.Get_Type() == token_type;
		}

		private bool Accept(KeyWords key)
		{
			return Correct_Keyword(key);
		}

		private bool Correct_Keyword(KeyWords desired)
		{
			return cur_token is KeyWord key && key.Get_Type_KeyWord() == desired;
		}

		private bool Correct_In_Startes()
		{
			return cur_token is KeyWord key && startes.Contains(key.Get_Type_KeyWord());
		}
	}
}
