using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssOptimizer.Domain;

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
					.Select(ConvertToUrl)
					.Select(uri => Task.Run(async () =>
				{

					var analyzer = new WebPageAnalyzer(CssStylesheets);

					var results = await analyzer.GetUnusedCssSelectors(uri);

					var formatResults = FormatResults(uri, results);



					Console.Write(formatResults);

					if (!String.IsNullOrWhiteSpace(options.OutputFile))
					{
						File.AppendAllText(options.GetOutputFileName(), formatResults);
					}

				})).ToList();

				await Task.WhenAll(tasks);

			}

						
		}

		private static Uri ConvertToUrl(string uriString)
		{
			var url = uriString;
			if (!Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
			{
				url = "http://" + uriString;
			}
			return new Uri(url);
		}


		private static string FormatResults(Uri url, Dictionary<Uri, IEnumerable<CssSelector>> results)
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
