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
			"Cascading StyleSheet compactor and syntax validator (version {0})\r\n\r\n"+
			"CssCompactor /IN:file [ /OUT:file ] [ /INFO:copyright ] [ /TIME:timeFormat ] [ /P ]\r\n\r\n"+
			"\t/IN:\tInput File Path\r\n"+
			"\t/OUT:\tOutput File Path\r\n"+
			"\t/INFO:\tCopyright label\r\n"+
			"\t/TIME:\tTimeStamp Format\r\n"+
			"\t/P\tPretty-Print the output (default is compact)\r\n\r\n"+
			"e.g. CssCompactor /IN:myFile.css /OUT:compacted/myFile.css /INFO:\"(c)2007 My CSS\" /TIME:\"'Compacted 'yyyy-MM-dd @ HH:mm\"";

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
				new ArgsMap<ArgType>("/INFO:", ArgType.Copyright),
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
					CssOptions.PrettyPrint|CssOptions.Overwrite : CssOptions.Overwrite;

				if (String.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
				{
					Console.Error.WriteLine(Program.Help, Assembly.GetExecutingAssembly().GetName().Version);
					return;
				}

				List<ParseException> errors;
				if (String.IsNullOrEmpty(outputFile))
				{
					errors = CssCompactor.Compact(inputFile, null, Console.Out, copyright, timeStamp, options);
				}
				else
				{
					errors = CssCompactor.Compact(inputFile, outputFile, copyright, timeStamp, options);
				}

				if (errors.Count > 0)
				{
					foreach (ParseException ex in errors)
					{
						Console.Error.WriteLine(ex.GetCompilerMessage());
					}
				}
				else
				{
					FileInfo inputInfo = new FileInfo(inputFile);
					FileInfo outputInfo = new FileInfo(outputFile);

					Console.WriteLine("Compacted \"{0}\" by {1:P1}", inputInfo.Name, (Decimal.One - (((decimal)outputInfo.Length)/((decimal)inputInfo.Length))));
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
			}
		}
	}
}
