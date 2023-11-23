using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Tokens
{
	class Identifier : Token
	{
		string lexeme;

		public Identifier(string lexeme) : base(type, position)
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
