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
		StreamReader file;
		string cur_line;
		Position cur_position;

		public File_Work(string path)
		{
			this.file = new StreamReader(path);
			cur_line = file.ReadLine();
			cur_position = new Position();
		}

		public string Get_Lexeme()
		{

		}

	}
}
