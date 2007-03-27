/*-----------------------------------------------------------------------*\
	Copyright (c) 2007 Stephen M. McKamey

	CssCompactor is distributed under the terms of an MIT-style license
	http://www.opensource.org/licenses/mit-license.php
\*-----------------------------------------------------------------------*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

using BuildTools.IO;

namespace BuildTools.CssCompactor
{
	class Program
	{
		#region Constants

		private const string Help =
			"Cascading StyleSheet compactor and syntax validator {0}\r\n\r\n"+
			"CssCompactor /IN:file [ /OUT:file ] [ /COPY:copyright ] [ /TIME:timeFormat ] [ /P ]\r\n\r\n"+
			"\t/IN:\tInput File Path\r\n"+
			"\t/OUT:\tOutput File Path\r\n"+
			"\t/COPY:\tCopyright label\r\n"+
			"\t/TIME:\tTimeStamp Format\r\n"+
			"\t/P\tPretty-Print the output\r\n\r\n"+
			"e.g. CssCompactor /IN:myFile.css /OUT:compacted/myFile.css /COPY:\"(c)2007 My CSS\" /TIME:\"'Compacted 'yyyy-MM-dd @ HH:mm\"";

		public enum ArgType
		{
			Empty,// need a default value
			InputFile,
			OutputFile,
			Copyright,
			TimeStamp,
			PrettyPrint
		}

		private static readonly ArgsTrie<ArgType> Mapping = new ArgsTrie<ArgType>(
			new ArgsMap<ArgType>[] {
				new ArgsMap<ArgType>("/IN:", ArgType.InputFile),
				new ArgsMap<ArgType>("/OUT:", ArgType.OutputFile),
				new ArgsMap<ArgType>("/COPY:", ArgType.Copyright),
				new ArgsMap<ArgType>("/TIME:", ArgType.TimeStamp),
				new ArgsMap<ArgType>("/P", ArgType.PrettyPrint),
			});

		#endregion Constants

		static void Main(string[] args)
		{
			try
			{
				Dictionary<ArgType,string> argList = Mapping.MapAndTrimPrefixes(args);

				string inputFile = argList.ContainsKey(ArgType.InputFile) ? argList[ArgType.InputFile] : null;
				string outputFile = argList.ContainsKey(ArgType.OutputFile) ? argList[ArgType.OutputFile] : null;
				string copyright = argList.ContainsKey(ArgType.Copyright) ? argList[ArgType.Copyright] : null;
				string timeStamp = argList.ContainsKey(ArgType.TimeStamp) ? argList[ArgType.TimeStamp] : null;
				CssOptions options = argList.ContainsKey(ArgType.PrettyPrint) ?
					CssOptions.PrettyPrint : CssOptions.None;

				if (String.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
				{
					Console.Error.WriteLine(Program.Help, Assembly.GetExecutingAssembly().GetName().Version);
					return;
				}

				TextWriter output = String.IsNullOrEmpty(outputFile) ?
					Console.Out : File.CreateText(outputFile);

				using (output)
				{
					CssParser parser = new CssParser(inputFile);
					parser.Write(output, options);
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
			}
		}
	}
}
