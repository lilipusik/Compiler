using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	enum Token_type { IDENTIFIER, CONST, KEYWORD, UNKNOWN, FUNCTION };

	abstract class Token
	{
		public static Token_type type;
		public static Position position;

		public Token(Token_type type, Position pos)
		{
			type = Token_type.UNKNOWN;
			position = new Position(pos.Get_Position().Item1, pos.Get_Position().Item2);
		}

		public Token_type Get_Type()
		{
			return type;
		}

		public override string ToString()
		{
			return $"token: {type}";
		}
	}
}
