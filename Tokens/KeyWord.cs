using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	enum KeyWords { 
		PROGRAM, BEGIN, END, VAR, IF, ELSE, THEN, WHILE, FOR, DO, TO,				// keywords
		LESS, GREATER, LESS_EQUAL, GREATER_EQUAL, EQUAL, NOT_EQUAL, AND, OR, NOT,   // conditions
		ASSIGN, PLUS, MINUS, MULTI, DIV, MOD,										// math operations
		LPAR, RPAR /* () */, LBRA, RBRA /* {} */, LSQR, RSQR /* [] */,				// brackets
		SEMICOLON /* ; */, COLON /* : */, COMMA /* , */, POINT /* . */,				// other operators
		CONST, INTEGER, FLOAT, STRING, BOOLEAN										// types
	};

	class KeyWord : Token
	{
		KeyWords keyword;

		public KeyWord(KeyWords key, Position position) : base(type, position)
		{
			this.keyword = key;
			type = Token_type.KEYWORD;
		}

		public override string ToString()
		{
			return $"token: {type} ({keyword})";
		}

		private static Dictionary<string, KeyWords> keywords = new Dictionary<string, KeyWords>
		{
			{ "program", KeyWords.PROGRAM },
			{ "begin", KeyWords.BEGIN },
			{ "end", KeyWords.END },
			{ "var", KeyWords.VAR },
			{ "if", KeyWords.IF },
			{ "else", KeyWords.ELSE },
			{ "then", KeyWords.THEN },
			{ "while", KeyWords.WHILE },
			{ "for", KeyWords.FOR },
			{ "do", KeyWords.DO },
			{ "to", KeyWords.TO },
			{ "<", KeyWords.LESS },
			{ ">", KeyWords.GREATER },
			{ "<=", KeyWords.LESS_EQUAL },
			{ ">=", KeyWords.GREATER_EQUAL },
			{ "=" , KeyWords.EQUAL },
			{ "<>" , KeyWords.NOT_EQUAL },
			{ "and", KeyWords.AND },
			{ "or" , KeyWords.OR },
			{ "not", KeyWords.NOT },
			{ ":=", KeyWords.ASSIGN },
			{ "+", KeyWords.PLUS },
			{ "-", KeyWords.MINUS },
			{ "*", KeyWords.MULTI },
			{ "div", KeyWords.DIV },
			{ "mod", KeyWords.MOD },
			{ "(", KeyWords.LPAR },
			{ ")", KeyWords.RPAR },
			{ "{", KeyWords.LBRA },
			{ "}", KeyWords.RBRA },
			{ "[", KeyWords.LSQR },
			{ "]", KeyWords.RSQR },
			{ ";", KeyWords.SEMICOLON },
			{ ":", KeyWords.COLON },
			{ ",", KeyWords.COMMA },
			{ ".", KeyWords.POINT },
			{ "const", KeyWords.CONST },
			{ "float", KeyWords.FLOAT },
			{ "integer", KeyWords.INTEGER },
			{ "string", KeyWords.STRING },
			{ "boolean", KeyWords.BOOLEAN }
		};

		public KeyWords Get_Type_KeyWord()
		{
			return keyword;
		}

		public static bool Is_KeyWord(string lexeme)
		{
			return keywords.ContainsKey(lexeme);
		}

		public static KeyWords Get_KeyWord(string lexeme)
		{
			return keywords[lexeme];
		}
	}
}
