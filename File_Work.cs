using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Compiler
{
	class File_Work
	{
		private StreamReader reader;
		private string cur_line;
		private Position cur_position;
		private List<char> separate_symbols = new List<char> { '<', '>', '=', ':', '+', '-', '*', '(', ')', '{', '}', '[', ']', ';', ':', ',', '.', ' ', '/', '"', '\t'};
		private List<char> double_symbols = new List<char> { '<', '>', '=', ':' };

		public File_Work(StreamReader reader)
		{
			this.reader = reader;
			cur_position = new Position();
			cur_line = reader.ReadLine();
		}

		public File_Work(StreamReader reader, Position position, string line)
		{
			this.reader = reader;
			this.cur_position = position;
			this.cur_line = line;
		}

		public StreamReader Get_StreamReader() { return reader; }
		
		public string Get_Line() { return cur_line; }

		public Tuple<int,int> Get_Position() { return cur_position.Get_Position(); }

		public void New_Line()
		{
			cur_line = reader.ReadLine();
			cur_position = new Position(cur_position.Get_Position().Item1 + 1);
		}

		public string Get_Lexeme()
		{
			if (cur_position.Get_Position().Item2 >= cur_line.Length) New_Line();

			bool flag_string = false;
			string lexeme = string.Empty;

			for (int i = cur_position.Get_Position().Item2; i < cur_line.Length; i++)
			{
				cur_position = new Position(cur_position.Get_Position().Item1, cur_position.Get_Position().Item2 + 1);

				if (!separate_symbols.Contains(cur_line[i]) || (flag_string && cur_line[i] != '"')) lexeme += cur_line[i];
				else
				{
					// token type => float
					if (cur_line[i] == '.' && int.TryParse(lexeme, out _))
					{
						lexeme += cur_line[i];
						continue;
					}

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
						if (lexeme == string.Empty) Get_Lexeme();
						else return lexeme;
					}

					// some lexeme
					if (lexeme.Length > 0)
					{
						cur_position = new Position(cur_position.Get_Position().Item1, cur_position.Get_Position().Item2 - 1);
						if (Char.IsDigit(lexeme[0]) && Char.IsDigit(lexeme[lexeme.Length - 1]) && lexeme.Contains('.')) // float
							lexeme = lexeme.Replace('.', ',');
						if (lexeme == string.Empty) Get_Lexeme();
						else return lexeme;
					}

					// move to next lexeme
					if (cur_line[i] == ' ' || cur_line[i] == '\t') continue;

					// lexeme from double operator
					if (i + 1 < cur_line.Length && double_symbols.Contains(cur_line[i]) && double_symbols.Contains(cur_line[i + 1]))
					{
						cur_position = new Position(cur_position.Get_Position().Item1, cur_position.Get_Position().Item2 + 1);
						lexeme = cur_line[i].ToString() + cur_line[i + 1].ToString();
						if (lexeme == string.Empty) Get_Lexeme();
						else return lexeme;
					}

					// single character lexeme
					lexeme = cur_line[i].ToString();
					if (lexeme == string.Empty) Get_Lexeme();
					else return lexeme;
				}
			}
			if (lexeme == string.Empty) Get_Lexeme();
			return lexeme;
		}
	}
}
