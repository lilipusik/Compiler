using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Analyzers
{
	internal class Lexer
	{
		private string lexeme;
		private Position position;

		public Lexer(File_Work file)
		{
			this.position = new Position(file.Get_Position().Item1, file.Get_Position().Item2);
			this.lexeme = file.Get_Lexeme();
		}

		public string Get_Lexeme()
		{
			return lexeme;
		}

		private bool Is_Function(ref Function_type type)
		{
			switch (lexeme)
			{
				case "writeln": type = Function_type.WRITELN; return true;
				case "readln": type = Function_type.READLN; return true;
				default: return false;
			} 
			
		}

		private bool Is_Identifier()
		{
			return lexeme.All(x => (x >= 65 && x <= 90) || (x >= 97 && x <= 122) || (x >= 48 && x <= 57) || x == 95) 
				&& !(lexeme[0] >= 48 && lexeme[0] <= 57);
		}

		public bool Is_Constant(ref Const_type type)
		{
			if (lexeme.StartsWith("\"") && lexeme.EndsWith("\""))
			{
				type = Const_type.STRING; return true;
			}

			if (lexeme == "True" || lexeme == "False" || lexeme == "true" || lexeme == "false")
			{
				type = Const_type.BOOLEAN; return true;
			}

			if (lexeme.All(x => char.IsDigit(x)))
			{
				type = Const_type.INTEGER; return true;
			}

			if (double.TryParse(lexeme, out _))
			{
				type = Const_type.FLOAT; return true;
			}

			return false;
		}
		
		public Token Get_Token()
		{
			if (KeyWord.Is_KeyWord(lexeme)) return new KeyWord(KeyWord.Get_KeyWord(lexeme), position);

			Const_type const_Type = Const_type.STRING;
			if (Is_Constant(ref const_Type)) return new Constant(const_Type, position);

			Function_type type_func = Function_type.READLN;
			if (Is_Function(ref type_func)) return new Function(type_func, position);

			if (Is_Identifier()) return new Identifier(lexeme, position);

			return new Unknown(lexeme, position);
		}
	}
}
