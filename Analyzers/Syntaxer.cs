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
		private StreamWriter writer; StreamReader reader;

		private File_Work file; Lexer lexer;

		private Position position;
		private string lexeme;
		private Token token;

		private List<bool> brackets = new List<bool>();

		public Syntaxer(StreamWriter sw, StreamReader sr)
		{
			writer = sw; reader = sr;

			file = new File_Work(reader);
			position = new Position();
			lexer = new Lexer(file);
			New_Token();
		}

		//----------------- Write result
		private void Print_Error(string message, string type)
		{
			writer.WriteLine(new Error(message, position, lexeme, type) + "\n");
		}

		private void Print_LexemeToken()
		{
			writer.WriteLine($"lexeme: {lexeme}\n{token} -> " +
				$"{new Position(position.Get_Position().Item1, position.Get_Position().Item2 - lexeme.Length)}\n");
		}

		//----------------- Get new token
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

		//----------------- BNF
		public void Program(Blocks order)
		{
			bool flag = true;
			while (flag)
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
						if (Accept(KeyWords.VAR)) Var();
						order = Blocks.BLOCK;
						break;

					case Blocks.BLOCK: 
						Block();
						order = Blocks.POINT;
						Next_Token(); 
						break;

					case Blocks.POINT:
						if (!Accept(KeyWords.POINT)) Print_Error("Not found point after block program", "Syntax error");
						flag = false; 
						break;
				}
		}

		private void String_Expression()
		{

		}

		private void Bool_Expression()
		{

		}

		private bool Factor()
		{
			if (Accept(Token_type.IDENTIFIER) || Accept(Token_type.CONST) || 
				Accept(new List<KeyWords>() { KeyWords.MULTI, KeyWords.DIV, KeyWords.MOD })) 
			{
				if (Accept(Token_type.IDENTIFIER) &&
					Semanter.Type_Variable(lexeme) != Const_type.INTEGER && Semanter.Type_Variable(lexeme) != Const_type.FLOAT ||
					Accept(Token_type.CONST) && 
					((Constant)token).Get_Const_Type() != Const_type.INTEGER && 
					((Constant)token).Get_Const_Type() != Const_type.FLOAT)
					
					Print_Error("Type mismatch in the expression", "Semantic error");

				Next_Token();
				return true; 
			}

			if (Accept(KeyWords.LPAR))
			{
				Next_Token();
				Math_Expression();
				if (!Accept(KeyWords.RPAR)) Print_Error("Not found closing bracket", "Syntax error");
				return true;
			}

			return false;
		}

		private bool Term()
		{
			while (Factor());
			return false;
		}

		private void Math_Expression()
		{
			while (Term() || Accept(new List<KeyWords>() { KeyWords.PLUS, KeyWords.MINUS })) Next_Token();
		}

		private void Choose_Expression(Const_type type)
		{
			switch(type)
			{
				case Const_type.INTEGER:
				case Const_type.FLOAT:
					Math_Expression();
					break;
				case Const_type.BOOLEAN:
					Bool_Expression();
					break;
				case Const_type.STRING:
					String_Expression();
					break;
			}
		}

		private void Expression()
		{
			if (Accept(KeyWords.ASSIGN))
			{
				Next_Token();
				if (Accept(Token_type.IDENTIFIER))
				{
					if (!Semanter.Has_Variable(lexeme)) { Print_Error("Not found variable definition", "Syntax error"); return; }
					Choose_Expression(Semanter.Type_Variable(lexeme));
				}
				else if (Accept(Token_type.CONST))
					Choose_Expression(((Constant)token).Get_Const_Type());
			}
			else Print_Error("Expression must have an assignment sign", "Syntax Error");
		}

		private bool Operator()
		{
			if (Accept(Token_type.IDENTIFIER))
			{
				Next_Token();
				Expression();
				return true;
			}
			else if (Accept(KeyWords.IF))
			{
				Next_Token();
				// условный оператор
				return true;
			}
			else if (Accept(KeyWords.WHILE))
			{
				Next_Token();
				// цикл 
				return true;
			}
			else if (Accept(KeyWords.SEMICOLON))
			{
				Next_Token();
				Operator();
				return true;
			}
			return false;
		}

		private void Block()
		{
			if (!Accept(KeyWords.BEGIN)) Print_Error("Uncorrect block start (not found begin)", "Syntax error");
			else
			{
				Next_Token();
				while (Operator()) Next_Token();
				if (!Accept(KeyWords.END)) Print_Error("Uncorrect block end (not found end)", "Syntax error");
			}
		}

		private void Var()
		{
			Next_Token();
			List<string> variables = new List<string>();

			while (Accept(Token_type.IDENTIFIER)) // new variable
			{
				if (Semanter.Has_Variable(lexeme)) Print_Error("Variable is already defined", "Semantic error");
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

		//----------------- Checking type
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
