using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	internal class Position
	{
		int line, symbol;

		public Position(int line, int symbol)
		{
			this.line = line;
			this.symbol = symbol;
		}

		public Position(int line)
		{
			this.line = line;
			this.symbol = 0;
		}

		public Position()
		{
			this.line = 0; this.symbol = 0;
		}

		public Tuple<int, int> Get_Position()
		{
			return new Tuple<int, int>(line, symbol);
		}

		public override string ToString()
		{
			return $"position: ({line}, {symbol})\n";
		}
	}
}
