using System;

namespace BuildTools.CssCompactor
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				string file = @"A:\Projects\Examples\Example.css";

				CssParser parser = new CssParser(file);

				parser.Write(Console.Out, CssOptions.None);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
			}
		}
	}
}
