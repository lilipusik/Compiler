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
		Position cur_position;
		List<char> separate_symbols = new List<char> { '<', '>', '=', ':', '+', '-', '*', '(', ')', '{', '}', '[', ']', ';', ':', ',', '.', ' ', '/', '"', '\t'};
		List<char> double_symbols = new List<char> { '<', '>', '=', ':' };

		public File_Work(string line, Position position)
		{
			this.cur_line = line;
			this.cur_position = new Position(position.Get_Position().Item1, 0);
		}

		public Position Get_Position() { return cur_position; }

		public string Get_Lexeme(Position position)
		{
			bool flag_string = false;
			string lexeme = string.Empty;
			for (int i = position.Get_Position().Item2; i < cur_line.Length; i++)
			{
				cur_position = new Position(cur_position.Get_Position().Item1, cur_position.Get_Position().Item2 + 1);

				if (!separate_symbols.Contains(cur_line[i]) || (flag_string && cur_line[i] != '"')) lexeme += cur_line[i];
				else
				{
					// token type => string
					if (!flag_string && cur_line[i] == '"')
					{
						flag_string = true;
						lexeme += cur_line[i];
						continue;
					}
					if (flag_string && cur_line[i] == '"')
					{
						lexeme += cur_line[i];
						return lexeme;
					}

					// some lexeme
					if (lexeme.Length > 0)
					{
						cur_position = new Position(cur_position.Get_Position().Item1, cur_position.Get_Position().Item2 - 1);
						return lexeme;
					}

					// move to next lexeme
					if (cur_line[i] == ' ' || cur_line[i] == '\t') break;

					// lexeme from double operator
					if (i + 1 < cur_line.Length && double_symbols.Contains(cur_line[i]) && double_symbols.Contains(cur_line[i + 1]))
					{
						cur_position = new Position(cur_position.Get_Position().Item1, cur_position.Get_Position().Item2 + 1);
						return cur_line[i].ToString() + cur_line[i + 1].ToString();
					}

					// single character lexeme
					return cur_line[i].ToString();
				}
			}
			return lexeme; // some lexeme
		}

	}
}
