using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	class Unknown : Token
	{
		string lexeme;

		public Unknown(string lexeme, Position position) : base(type, position)
		{
			this.lexeme = lexeme;
			type = Token_type.UNKNOWN;
		}

		public override string ToString()
		{
			return $"token: {type}";
		}
	}
}
