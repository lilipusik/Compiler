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

	class Syntaxer
	{
		StreamWriter writer; StreamReader reader;

		File_Work file;
		Lexer lexer;

		string line;
		Position position;
		string lexeme;
		Token token;
		List<KeyWords> startes = new List<KeyWords>();


		public Syntaxer(StreamWriter sw, StreamReader sr)
		{
			writer = sw; reader = sr;

			file = new File_Work(reader);
			position = new Position();
			lexer = new Lexer(file);

			New_Token();
		}

		private void Print_Error(string message, string type)
		{
			writer.WriteLine(new Error(message, position, lexeme, type));
		}

		private void Print_LexemeToken()
		{
			writer.WriteLine("lexeme: " + lexeme + "\n" + token + " -> " + position + "\n");
		}

		public void Next_Token()
		{
			position = new Position(file.Get_Position().Item1, file.Get_Position().Item2);

			file = new File_Work(reader, position, file.Get_Line());
			lexer = new Lexer(file);
			New_Token();
		}

		private void New_Token()
		{
			lexeme = lexer.Get_Lexeme();
			reader = file.Get_StreamReader();

			while (lexeme == string.Empty)
			{
				position = new Position(position.Get_Position().Item1, position.Get_Position().Item2 + 1);
				file = new File_Work(reader, position, file.Get_Line());
				lexer = new Lexer(file);
				lexeme = lexer.Get_Lexeme();
				reader = file.Get_StreamReader();
			}

			token = lexer.Get_Token();

			Print_LexemeToken();
		}

		public void Program(Blocks order)
		{
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

					case Blocks.VAR: flag = false; break;

					case Blocks.BLOCK: flag = false; break;

					case Blocks.POINT: flag = false; break;
				}
			}
		}

		private bool Accept(Token_type token_type)
		{
			return token.Get_Type() == token_type;
		}

		private bool Accept(KeyWords key)
		{
			return Correct_Keyword(key);
		}

		private bool Correct_Keyword(KeyWords desired)
		{
			return token is KeyWord key && key.Get_Type_KeyWord() == desired;
		}

		private bool Correct_In_Startes()
		{
			return token is KeyWord key && startes.Contains(key.Get_Type_KeyWord());
		}
	}
}
