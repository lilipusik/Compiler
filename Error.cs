using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	class Error
	{
		string message;
		string type;
		Position position;
		string lexeme;

		public Error(string message, Position position, string lexeme, string type)
		{
			this.message = message;
			this.position = position;
			this.lexeme = lexeme;
			this.type = type;
		}

		public override string ToString()
		{
			return $"!!! Error: {message} ({type}) -> {position} -> {lexeme}";
		}
	}
}
