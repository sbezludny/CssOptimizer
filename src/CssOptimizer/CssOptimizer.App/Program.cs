using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssOptimizer.Domain;
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

			MainAsync(args).Wait();


#if DEBUG
			sw.Stop();

			Console.WriteLine(sw.ElapsedMilliseconds);
#endif
			
#if DEBUG
			Console.ReadKey();
#endif
			
		}

		private static async Task MainAsync(string[] args)
		{
			var options = new Options();

			if (CommandLine.Parser.Default.ParseArguments(args, options))
			{

				if (!String.IsNullOrWhiteSpace(options.OutputFile))
				{
					File.Create(options.GetOutputFileName());
				}

				var tasks = options.Urls
					.Select(UrlHelper.CreateInvariantToScheme)
					.Select(uri => Task.Run(async () =>
				{

					var analyzer = new WebPageAnalyzer(CssStylesheets);

					var results = await analyzer.GetUnusedCssSelectors(uri);

					var formatedResults = FormatResults(uri, results);



					Console.Write(formatedResults);

					if (!String.IsNullOrWhiteSpace(options.OutputFile))
					{
						File.AppendAllText(options.GetOutputFileName(), formatedResults);
					}

				})).ToList();

				await Task.WhenAll(tasks);

			}
		}

		private static string FormatResults(Uri url, IEnumerable<KeyValuePair<Uri, IEnumerable<CssSelector>>> results)
		{
			var sb = new StringBuilder();

			foreach (var ruleSet in results)
			{
				sb.AppendFormat("{0} ---> {1}\r\n", url, ruleSet.Key);
				sb.AppendLine("=====================");
				foreach (var selector in ruleSet.Value)
				{
					sb.AppendLine(selector.ToString());
				}

				sb.AppendLine("=====================");
			}

			return sb.ToString();
		}
	}
}
