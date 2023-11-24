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

		public Constant(Const_type const_type, string lexeme, Position position) : base(type, position)
		{
			this.const_type = const_type;
			this.lexeme = lexeme;
			type = Token_type.CONST;
		}

		public Const_type Get_Const_Type()
		{
			return const_type;
		}

		public override string ToString()
		{
			return $"token: {type} ({const_type})";
		}
	}
}
