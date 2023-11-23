using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Tokens
{
	class KeyWord : Token
	{
		string lexeme;

		public KeyWord(string lexeme) : base(type, position)
		{
			this.lexeme = lexeme;
			type = Token_type.KEYWORD;
		}

		public override string ToString()
		{
			return $"token: {type}";
		}
	}
}
