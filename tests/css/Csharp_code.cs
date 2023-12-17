
using System;
namespace Compiler
{
	class Program
	{
		static void Main()
		{
			double a;
			double b;
			int c;
			int i;
			string d;
			bool e;
			bool f;
			
			Console.WriteLine("Write a:");
			a = double.Parse(Console.ReadLine());

			b = 8+a*(7.4+7);
			Console.WriteLine("Write d:");
			d = Console.ReadLine();

			d = "stroka"+d;
			Console.WriteLine("\nResult:");
			Console.WriteLine(b);
			Console.WriteLine(d);
			Console.ReadKey();
		}
	}
}
