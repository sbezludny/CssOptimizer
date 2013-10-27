using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CssOptimizer.Domain.Utils;
using HtmlAgilityPack;

namespace CssOptimizer.Domain
{
	public class WebPageAnalyzer
	{
		private readonly CssStylesheets _stylesheets;

		public WebPageAnalyzer(CssStylesheets stylesheets)
		{
			_stylesheets = stylesheets;
		}

		public async Task<IDictionary<Uri, IEnumerable<CssSelector>>> GetUnusedCssSelectors(Uri pageUrl)
		{
			var result = new ConcurrentDictionary<Uri, IEnumerable<CssSelector>>();

			var html = ParseHtml(await WebClientHelper.DownloadStringAsync(pageUrl));

			AnalyzeInternalStyles(pageUrl, html, result);

			await AnalyzeExternalStyles(pageUrl, html, result);

			return result;
		}

		private async Task AnalyzeExternalStyles(Uri pageUrl, HtmlDocument html, ConcurrentDictionary<Uri, IEnumerable<CssSelector>> result)
		{
			var cssUrls = html
				.GetExternalCssLinks()
				.Select(href => UrlHelper.CreateFromHref(pageUrl, href))
				.ToList();

			var tasks = cssUrls.Select(cssUrl => Task.Run(async () =>
			{
				var stylesheet = await _stylesheets.GetOrDownload(cssUrl);

				AnalyzeCssStylesheet(stylesheet, html, result);
			})).ToList();

			await Task.WhenAll(tasks);

		}

		private static void AnalyzeInternalStyles(Uri pageUrl, HtmlDocument html, ConcurrentDictionary<Uri, IEnumerable<CssSelector>> result)
		{
			if (String.IsNullOrWhiteSpace(html.GetInlineStyles())) 
				return;
			
			
			var inlineCss = html.GetInlineStyles();

			var styleSheet = new CssStylesheet(pageUrl, inlineCss);

			AnalyzeCssStylesheet(styleSheet, html, result);
		}

		private static HtmlDocument ParseHtml(string htmlSource)
		{
			var html = new HtmlDocument();
			html.LoadHtml(htmlSource);

			return html;
		}

		private static void AnalyzeCssStylesheet(CssStylesheet stylesheet, HtmlDocument html, ConcurrentDictionary<Uri, IEnumerable<CssSelector>> result)
		{
			var unusedSelectors = stylesheet.Selectors
				.AsParallel()
				.Where(s => !html.IsSelectorInUse(s))
				.ToList();

			if (unusedSelectors.Any())
			{
				result.TryAdd(stylesheet.Url, unusedSelectors);
			}
		}

		

	}
}
