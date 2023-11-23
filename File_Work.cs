using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Compiler
{
	internal class File_Work
	{
		string cur_line;
		char cur_symbol;
		Position cur_position;
		List<char> separate_symbols = new List<char> { '<', '>', '=', ':', '+', '-', '*', '(', ')', '{', '}', '[', ']', ';', ':', ',', '.', ' '};

		public Position Get_Position()
		{
			return cur_position;
		}

		public File_Work(string line)
		{
			cur_line = line;
			cur_position = new Position();
		}

		private char Next_Symbol()
		{
			cur_symbol = cur_line[cur_position.Get_Position().Item2];
			cur_position = new Position(cur_position.Get_Position().Item1, cur_position.Get_Position().Item2 + 1);
			return cur_symbol;
		}

		public string Get_Lexeme()
		{
			string lexeme = string.Empty;
			while (!separate_symbols.Contains(cur_symbol) && cur_position.Get_Position().Item2 < cur_line.Length) 
			{
				lexeme += Next_Symbol();
			};
			return lexeme;
		}

	}
}
