using System;

namespace Compiler
{
	class Generator
	{
		string lexeme;
		string expression;

		public static string Result { get; private set; } = string.Empty;

		public Generator(string lexeme)
		{
			this.lexeme = lexeme;
			expression = Semanter.Get_Value(lexeme);
		}

		public void Expression_Calculator()
		{
			if (expression != string.Empty)
			{
				try
				{
					if (Semanter.Type_Variable(lexeme) == Const_type.STRING) String_Expression();
					else Math_Expression();
				}
				catch 
				{
					Result = "error";
				}
			}
		}

		private void Math_Expression()
		{
			StringToFormula math_ = new StringToFormula();
			Result += math_.Eval(expression).ToString() + "\n";
		}

		private void String_Expression()
		{
			Result += string.Join("", expression.Split('"', '+')) + "\n";
		}
	}
}
