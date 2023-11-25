using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	class Error
	{
		string message;
		Position position;
		string lexeme;

		public Error(string message, Position position, string lexeme)
		{
			this.message = message;
			this.position = position;
			this.lexeme = lexeme;
		}

		public override string ToString()
		{
			return $"\n!!! Error: {message} -> {position} -> {lexeme}";
		}
	}
}
