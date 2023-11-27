using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	class Identifier : Token
	{
		string lexeme;

		public Identifier(string lexeme, Position position) : base(type, position)
		{
			this.lexeme = lexeme;
			type = Token_type.IDENTIFIER;
		}

		public override string ToString()
		{
			return $"token: {type}";
		}
	}
}
