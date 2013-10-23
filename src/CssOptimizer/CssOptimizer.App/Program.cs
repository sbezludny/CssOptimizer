using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CssOptimizer.Domain;
using HtmlAgilityPack;

namespace CssOptimizer.App
{
	class Program
	{
		static void Main(string[] args)
		{

			var url = new Uri("http://uawebchallenge.com/");

			var analyzer = new WebPageAnalyzer();

			var results = analyzer.GetUnusedCssSelectors(url);

			DisplayResults(results);

			Console.ReadKey();
		}

		private static void DisplayResults(Dictionary<Uri, IEnumerable<string>> results)
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
