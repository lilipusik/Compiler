using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	enum Function_type { READLN, WRITELN, MATH };
	class Function : Token
	{
		private Function_type func_type;

		public Function(Function_type func_type, Position position) : base(type, position)
		{
			this.func_type = func_type;
			type = Token_type.FUNCTION;
		}

		public Function_type GetFunc_Type()
		{
			return func_type;
		}

		public override string ToString()
		{
			return $"token: {type} ({func_type})";
		}
	}
}
