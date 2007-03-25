/*------------------------------------------------------*\
	Copyright (c) 2007 Stephen M. McKamey

	CssCompactor is open-source under The MIT License
	http://www.opensource.org/licenses/mit-license.php
\*------------------------------------------------------*/

using System;
using System.IO;

namespace BuildTools.CssCompactor
{
	class Program
	{
		private const string Help =
			"Cascading StyleSheet compactor and syntax validator\r\n\r\n"+
			"CssCompactor /IN:file [ /OUT:file ] [ /COPY:copyright ] [ /TIME:timeFormat ] [ /P ]\r\n\r\n"+
			"\t/IN:\tInput File Path\r\n"+
			"\t/OUT:\tOutput File Path\r\n"+
			"\t/COPY:\tCopyright label\r\n"+
			"\t/TIME:\tTimeStamp Format\r\n"+
			"\t/P\tPretty-Print the output\r\n\r\n"+
			"e.g. CssCompactor /IN:myFile.css /OUT:myCompactFile.css /COPY:\"(c)2007 My CSS\" /TIME:\"'Compacted 'yyyy-MM-dd @ HH:mm\"";

		static void Main(string[] args)
		{
			try
			{
				string inputFile = (args.Length > 0) ? args[0] : null;
				string outputFile = (args.Length > 1) ? args[1] : null;
				string copyright = (args.Length > 2) ? args[2] : null;
				string timestamp = (args.Length > 3) ? args[3] : null;
				string prettyPrint = (args.Length > 3) ? args[3] : null;

				if (String.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
				{
					Console.Error.WriteLine(Program.Help);
					return;
				}

				TextWriter output = String.IsNullOrEmpty(outputFile) ?
					Console.Out : File.CreateText(outputFile);

				using (output)
				{
					CssParser parser = new CssParser(inputFile);
					parser.Write(output, CssOptions.None);
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
			}
		}
	}
}
