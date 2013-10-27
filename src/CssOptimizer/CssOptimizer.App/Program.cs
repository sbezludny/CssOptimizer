using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine.Text;
using CssOptimizer.Domain;
using CssOptimizer.Domain.Analysis;
using CssOptimizer.Domain.Utils;

namespace CssOptimizer.App
{
	class Program
	{
		private static readonly CssStylesheets CssStylesheets = new CssStylesheets();

		static void Main(string[] args)
		{

#if DEBUG
			var sw = new Stopwatch();
			sw.Start();
#endif
			try
			{
				var options = new Options();

				if (CommandLine.Parser.Default.ParseArguments(args, options))
				{
					var processor = new Processor(CssStylesheets);

					if (options.SiteAnalysisMode)
					{
						processor.AnalyzeWebSite(options).Wait();
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

#if DEBUG
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
#endif	
		}

		
	}
}
