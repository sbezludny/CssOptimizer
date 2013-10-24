using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CssOptimizer.Domain;

namespace CssOptimizer.App
{
	class Program
	{
		private static readonly CssStylesheets CssStylesheets = new CssStylesheets();

		static void Main(string[] args)
		{

			var url = new Uri(args[0]);

			var analyzer = new WebPageAnalyzer(CssStylesheets);

			var results = analyzer.GetUnusedCssSelectors(url);

			DisplayResults(results);

			Console.ReadKey();
		}

		

		private static void DisplayResults(Dictionary<Uri, IEnumerable<CssSelector>> results)
		{
			foreach (var pageRuleSet in results)
			{
				Console.WriteLine(pageRuleSet.Key.ToString());
				Console.WriteLine("=====");
				foreach (var rule in pageRuleSet.Value)
				{
					Console.WriteLine(rule);
				}

				Console.WriteLine("=====");
			}
		}
	}
}
