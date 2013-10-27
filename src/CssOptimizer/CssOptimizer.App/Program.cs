using System;
using System.Collections.Generic;
using System.Diagnostics;
using CssOptimizer.Domain;

namespace CssOptimizer.App
{
	class Program
	{
		static void Main(string[] args)
		{

			var sw = new Stopwatch();
			sw.Start();
			try
			{
				var options = new Options();

				if (CommandLine.Parser.Default.ParseArguments(args, options))
				{
					var processor = new Processor();

					if (options.SiteAnalysisMode)
					{
						processor.AnalyzeWebSites(options).Wait();
					}
					else
					{
						processor.AnalyzeWebPages(options).Wait();
					}
				}

				//todo:handle no opts
			}
			catch (AggregateException ex)
			{
				foreach (var exception in ex.InnerExceptions)
				{
					Console.Error.WriteLine(exception.Message);
				}
			}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
		}

		
	}
}
