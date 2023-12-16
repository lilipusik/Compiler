using Compiler.Analyzers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	class Syntaxer
	{
		private StreamWriter writer; StreamReader reader;

		private File_Work file; private Lexer lexer;

		private Position position;
		private string lexeme;
		private Token token;

		public static int Count_Errors { get; private set; } = 0;

		private string expression;

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
			Count_Errors++;
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
		public void Program()
		{
			if (!Accept(KeyWords.PROGRAM)) Print_Error("Uncorrect program start", "Syntax error");
			else
			{
				Next_Token();
				if (!Accept(Token_type.IDENTIFIER)) Print_Error("Uncorrect program name", "Syntax error");

				Next_Token();
				if (!Accept(KeyWords.SEMICOLON)) Print_Error("Not found semicolon after program name", "Syntax error");

				Next_Token();
				if (Accept(KeyWords.VAR)) Var();

				Block();

				Next_Token();
				if (!Accept(KeyWords.POINT)) Print_Error("Not found point after block program", "Syntax error");
			}
		}

		private bool String_Term()
		{
			if (Accept(Token_type.IDENTIFIER) || Accept(Token_type.CONST))
			{
				if (Accept(Token_type.IDENTIFIER) &&
					Semanter.Type_Variable(lexeme) != Const_type.STRING ||
					Accept(Token_type.CONST) && ((Constant)token).Get_Const_Type() != Const_type.STRING)

					Print_Error("Type mismatch in the expression", "Semantic error");

				if (Accept(Token_type.IDENTIFIER) && !Semanter.Is_Assignment(lexeme)) Print_Error("Using a variable without a value", "Semantic error");

				return true;
			}
			return false;
		}

		private void String_Expression()
		{
			while (String_Term() || Accept(KeyWords.PLUS)) { Next_Token(); expression += lexeme; }
			expression = expression.Substring(0, expression.Length - lexeme.Length);
		}

		private bool Bool_Factor()
		{
			if (Accept(Token_type.IDENTIFIER) || Accept(Token_type.CONST) || Accept(KeyWords.AND))
			{
				if (Accept(Token_type.IDENTIFIER) && !Semanter.Is_Assignment(lexeme)) Print_Error("Using a variable without a value", "Semantic error");

				Next_Token();

				return true;
			}

			return false;
		}

		private bool Bool_Temp()
		{
			while (Bool_Factor());
			return false;
		}

		private bool Simple_Bool_Expr()
		{
			while (Bool_Temp() || Accept(KeyWords.OR)) Next_Token();
			return false;
		}

		private void Bool_Expression()
		{
			while (Simple_Bool_Expr() || Accept(new List<KeyWords>() { KeyWords.LESS, KeyWords.LESS_EQUAL, KeyWords.EQUAL, KeyWords.NOT_EQUAL,
				KeyWords.GREATER_EQUAL, KeyWords.GREATER})) Next_Token();
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

				if (Accept(Token_type.IDENTIFIER) && !Semanter.Is_Assignment(lexeme)) Print_Error("Using a variable without a value", "Semantic error");

				Next_Token();
				expression += lexeme;
				return true; 
			}

			if (Accept(KeyWords.LPAR))
			{
				Next_Token();
				expression += lexeme;
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
			while (Term() || Accept(new List<KeyWords>() { KeyWords.PLUS, KeyWords.MINUS })) { Next_Token(); expression += lexeme; }
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

		private void Expression(string lex, bool flag)
		{
			if (Accept(KeyWords.ASSIGN))
			{
				Next_Token();
				expression += lexeme;
				if (Accept(Token_type.IDENTIFIER))
				{
					if (!Semanter.Has_Variable(lex)) Print_Error("Not found variable definition", "Syntax error");
					else Choose_Expression(Semanter.Type_Variable(lex));
				}
				else if (Accept(Token_type.CONST))
					Choose_Expression(((Constant)token).Get_Const_Type());
			}
			else if (flag)
			{
				if (!Semanter.Has_Variable(lex)) Print_Error("Not found variable definition", "Syntax error");
				else Choose_Expression(Semanter.Type_Variable(lex));
				Next_Token();
				expression += lexeme;
				if (Accept(Token_type.IDENTIFIER))
				{
					if (!Semanter.Has_Variable(lex)) Print_Error("Not found variable definition", "Syntax error");
					else Choose_Expression(Semanter.Type_Variable(lex));
				}
				else if (Accept(Token_type.CONST))
					Choose_Expression(((Constant)token).Get_Const_Type());
			}
			else Print_Error("Not found assignment in expression", "Syntax error");
		}

		private bool Operator()
		{
			expression = string.Empty;
			string lex = lexeme;
			if (Accept(Token_type.FUNCTION))
			{
				Function_type type = ((Function)token).GetFunc_Type();
				Next_Token();
				if (!Accept(KeyWords.LPAR)) Print_Error("Not found opening bracket after function", "Syntax error");
				else
				{
					Next_Token();
					if (type == Function_type.READLN)
					{
						expression = Console.ReadLine();
						Const_type const_ = Semanter.Type_Variable(lexeme);
						switch (const_)
						{
							case Const_type.INTEGER:
								if (!int.TryParse(expression, out _))
									Print_Error("Data entry error", "Semantic error");
								break;
							case Const_type.FLOAT:
								double p;
								if (!double.TryParse(expression, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out p))
									Print_Error("Data entry error", "Semantic error");
								else expression = p.ToString();
								break;
						}
						Semanter.New_Assignment(lexeme, expression);
						Generator.Add_Code(type, lexeme, const_);
					}
					else
					{
						Generator generator = new Generator(lexeme);
						generator.Expression_Calculator();
						if (Generator.Result == "error") Print_Error("Calculation error", "Semantic error");
						else if (Generator.Result != string.Empty) Generator.Writeline();
					}
					Next_Token();
					if (!Accept(KeyWords.RPAR)) Print_Error("Not found closing bracket after function", "Syntax error");
					Next_Token();
					if (!Accept(KeyWords.SEMICOLON)) Print_Error("Not found semicolon after function", "Syntax error");
				}
				return true;
			}
			else if (Accept(Token_type.IDENTIFIER))
			{
				if (!Semanter.Has_Variable(lex)) Print_Error("Not found variable definition", "Syntax error");
				Next_Token();
				Expression(lex, false);
				Semanter.New_Assignment(lex, expression);
				return true;
			}
			else if (Accept(KeyWords.IF))
			{
				Next_Token();
				Expression(lexeme, true);
				if (!Accept(KeyWords.THEN)) { Print_Error("Not found keyword then after bool expression", "Syntax error"); return false; }
				Next_Token();
				if (!Accept(KeyWords.BEGIN)) { Print_Error("Not found begin after bool expression", "Syntax error"); return false; }
				Next_Token();
				while (Operator()) Next_Token();
				if (!Accept(KeyWords.END)) Print_Error("Not found end after if block", "Syntax error");
				Next_Token();
				if (Accept(KeyWords.ELSE))
				{
					Next_Token();
					if (!Accept(KeyWords.BEGIN)) { Print_Error("Not found begin after bool expression", "Syntax error"); return false; }
					Next_Token();
					while (Operator()) Next_Token();
					if (!Accept(KeyWords.END)) Print_Error("Not found end after else block", "Syntax error");
					Next_Token();
					if (!Accept(KeyWords.SEMICOLON)) Print_Error("Not found semicolon after else block end", "Syntax error");
				}
				return true;
			}
			else if (Accept(KeyWords.WHILE))
			{
				Next_Token();
				Expression(lexeme, true);
				if (!Accept(KeyWords.DO)) Print_Error("Not found do after while expression", "Syntax Error");
				Next_Token();
				if (!Accept(KeyWords.BEGIN)) { Print_Error("Not found begin after while expression", "Syntax error"); return false; }
				Next_Token();
				while (Operator()) Next_Token();
				if (!Accept(KeyWords.END)) Print_Error("Not found end after while block", "Syntax error");
				Next_Token();
				if (!Accept(KeyWords.SEMICOLON)) Print_Error("Not found semicolon after while block end", "Syntax error");
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

			Generator.Traslate_VAR();
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
