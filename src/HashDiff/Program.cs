﻿namespace HashDiff
{
	using System;
	using System.IO;
	using HashDiff.Core;
	using HashDiff.Core.Output;

	class Program
	{
		const int EXITCODE_ERROR = 2;
		const int EXITCODE_DIFF = 1;

		static void Main(string[] args)
		{
			OutputLogo();

			var options = new Options();
			if (!CommandLine.Parser.Default.ParseArguments(args, options))
			{
				Console.WriteLine("No checksum hash files have been specified.");
				Environment.Exit(EXITCODE_ERROR);
			}

			var file1 = new FileInfo(options.File1);
			var file2 = new FileInfo(options.File2);

			CheckFile(file1);
			CheckFile(file2);

			Console.WriteLine("File 1 = {0}", file1.Name);
			Console.WriteLine("File 2 = {0}", file2.Name);
			Console.WriteLine();

			var calc = new DifferenceCalculator(file1.FullName, file2.FullName)
			{
				IgnoreCase = options.IgnoreCase,
				Verbose = options.Verbose
			};

			var diff = calc.GetDifferences();

			if (!string.IsNullOrWhiteSpace(options.OutputFile))
			{
				WriteOutputFile(options.OutputFile, diff);
			}

			if (diff.HasDifferences())
			{
				if (options.Verbose)
				{
					Console.WriteLine(diff);
					Console.WriteLine("{0} and {1} are different", options.File1, options.File2);
				}

				if (options.ExitWithErrorOnDifference)
				{
					Environment.Exit(EXITCODE_DIFF);
				}
			}
			else
			{
				Console.WriteLine("No differences found.");
			}

			Console.Read();
		}

		static void CheckFile(FileInfo file)
		{
			if (!file.Exists)
			{
				Console.WriteLine("'{0}' does not exist.", file.Name);
				Environment.Exit(EXITCODE_ERROR);
			}

			if ((file.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
			{
				Console.WriteLine("'{0}' is a directory.", file.Name);
				Environment.Exit(EXITCODE_ERROR);
			}
		}

		static void WriteOutputFile(string filename, Differences diff)
		{
			var builder = BuilderFactory.Create(filename);
			builder.Build(filename, diff);
		}

		static void OutputLogo()
		{
			Console.WriteLine(@"
  _    _           _     _____  _  __  __ 
 | |  | |         | |   |  __ \(_)/ _|/ _|
 | |__| | __ _ ___| |__ | |  | |_| |_| |_ 
 |  __  |/ _` / __| '_ \| |  | | |  _|  _|
 | |  | | (_| \__ \ | | | |__| | | | | |  
 |_|  |_|\__,_|___/_| |_|_____/|_|_| |_|  
");
		}
	}
}