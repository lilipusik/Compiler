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

		Position position;
		string lexeme;
		Token token;


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
			writer.WriteLine(new Error(message, position, lexeme, type) + "\n");
		}

		private void Print_LexemeToken()
		{
			writer.WriteLine($"lexeme: {lexeme}\n{token} -> " +
				$"{new Position(position.Get_Position().Item1, position.Get_Position().Item2 - lexeme.Length)}\n");
		}

		public void Next_Token()
		{
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
			position = new Position(file.Get_Position().Item1, file.Get_Position().Item2);
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
						if (!Accept(Token_type.IDENTIFIER)) Print_Error("Uncorrect program name", "Syntax error");
						order = Blocks.SEMICOLON;
						Next_Token();
						break;

					case Blocks.SEMICOLON:
						if (!Accept(KeyWords.SEMICOLON)) Print_Error("Not found semicolon after program name", "Syntax error");
						order = Blocks.VAR;
						Next_Token();
						break;

					case Blocks.VAR:
						if (Accept(KeyWords.VAR)) VAR();
						order = Blocks.BLOCK;
						Next_Token();
						break;

					case Blocks.BLOCK: 
						Block();
						order = Blocks.POINT;
						Next_Token(); 
						break;

					case Blocks.POINT:
						if (!Accept(KeyWords.POINT)) Print_Error("Not found point after block program", "Synatax error");
						flag = false; 
						break;
				}
			}
		}

		private void Block()
		{

		}

		private void VAR()
		{
			Next_Token();
			List<string> variables = new List<string>();

			while (Accept(Token_type.IDENTIFIER)) // new variable
			{
				if (Semanter.Has_Variable(lexeme)) Print_Error("Variable is already defined", "Sematic error");
				else variables.Add(lexeme);

				Next_Token();
				if (Accept(KeyWords.COMMA)) { Next_Token(); continue; }
				if (Accept(KeyWords.COLON)) // type
				{
					Next_Token();
					if (Accept(new List<KeyWords>() { KeyWords.BOOLEAN, KeyWords.INTEGER, KeyWords.FLOAT, KeyWords.STRING }))
					{
						Semanter.Add_Variables(variables, Const_Type());
						variables.Clear();
					}
					else
					{
						variables.Clear();
						Print_Error("Uncorrect variable type", "Syntax error");
					}
					Next_Token();
					if (!Accept(KeyWords.SEMICOLON)) Print_Error("Not found semicolon after variable type", "Syntax error");
				}
				Next_Token();
			}
		}

		private bool Accept(List<KeyWords> keys)
		{
			return token is KeyWord key && keys.Contains(key.Get_Type_KeyWord());
		}

		private bool Accept(Token_type token_type)
		{
			return token.Get_Type() == token_type;
		}

		private bool Accept(KeyWords key)
		{
			return token is KeyWord word && word.Get_Type_KeyWord() == key;
		}

		private Const_type Const_Type()
		{
			return Constant.Get_Const_Type(((KeyWord)token).Get_Type_KeyWord());
		}
	}
}
