using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CssOptimizer.Domain
{
	public static class HtmlDocumentExtensions
	{
		

		public static IEnumerable<string> GetExternalCssSources(this HtmlDocument html)
		{
			return html.DocumentNode
				.SelectNodes("//link[@href]")
				.Select(link => link.Attributes["href"].Value)
				.Distinct()
				.ToList();
		}

		public static string GetInlineCss(this HtmlDocument html)
		{
			return html.DocumentNode
				.SelectNodes("//style")
				.Aggregate(String.Empty, (inline, node) => inline + node.InnerText.Trim());
		}
	}
}
