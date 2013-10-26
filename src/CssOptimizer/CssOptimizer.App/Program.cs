using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using CssOptimizer.Domain;

namespace CssOptimizer.App
{
	class Program
	{
		private static readonly CssStylesheets CssStylesheets = new CssStylesheets();

		static void Main(string[] args)
		{

			var url = new Uri(args[0]);

			var sw = new Stopwatch();
			sw.Start();
			var analyzer = new WebPageAnalyzer(CssStylesheets);

			var results = analyzer.GetUnusedCssSelectors(url);

			var formatResults = FormatResults(results.Result);
			sw.Stop();

			Debug.WriteLine(sw.ElapsedMilliseconds);
			
			Console.Write(formatResults);

#if DEBUG
			File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) , "results.txt"), formatResults);
#endif

			Console.ReadKey();
		}

		

		private static string FormatResults(Dictionary<Uri, IEnumerable<CssSelector>> results)
		{
			var sb = new StringBuilder();

			foreach (var pageRuleSet in results)
			{
				sb.AppendLine(pageRuleSet.Key.ToString());
				sb.AppendLine("=====");
				foreach (var selector in pageRuleSet.Value)
				{
					sb.AppendLine(selector.ToString());
				}

				sb.AppendLine("=====");
			}

			return sb.ToString();
		}
	}
}
