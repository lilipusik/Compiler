using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Tokens
{
	enum Const_type { STRING, INTEGER, BOOLEAN, FLOAT };

	class Constant : Token
	{
		string lexeme;
		Const_type const_type;

		public Constant(string lexeme) : base(type, position)
		{
			this.lexeme = lexeme;
			type = Token_type.CONST;
		}

		public override string ToString()
		{
			return $"token: {type} ({const_type})";
		}
	}
}
