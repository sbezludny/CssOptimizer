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
		private static HtmlNodeCollection SelectNodeCollection(this HtmlNode node, string xPath)
		{
			return node.SelectNodes(xPath) ?? new HtmlNodeCollection(null);
		}

		public static IEnumerable<string> GetExternalCssLinks(this HtmlDocument html)
		{
			return html.DocumentNode
				.SelectNodeCollection("//link[@href]")
				.Select(link => link.Attributes["href"].Value)
				.Distinct()
				.ToList();
		}

		public static string GetInlineCss(this HtmlDocument html)
		{
			return html.DocumentNode
				.SelectNodeCollection("//style")
				.Aggregate(String.Empty, (inline, node) => inline + node.InnerText.Trim());
		}

		public static bool HasElementsWithSelector(this HtmlDocument html, CssSelector selector)
		{
			return html.DocumentNode.SelectNodes(selector.ToXPath()) != null;
		}
	}
}
