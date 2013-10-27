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

			var html = await GetHtmlDocumentAsync(pageUrl);

			var cssUrls = html.GetExternalCssLinks()
				.Select(href => UrlHelper.CreateFromHref(pageUrl, href)).ToList();

			var tasks = cssUrls.Select(cssUrl => Task.Run(async () =>
			{
				var stylesheet = await _stylesheets.GetOrDownload(cssUrl);

				AnalyzeCssStylesheet(stylesheet, html, result);
			})).ToList();

			if (!String.IsNullOrWhiteSpace(html.GetInlineCss()))
			{
				var inlineCss = html.GetInlineCss();

				var styleSheet = new CssStylesheet(pageUrl, inlineCss);

				AnalyzeCssStylesheet(styleSheet, html, result);
			}

			await Task.WhenAll(tasks);

			return result;


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


		private async Task<HtmlDocument> GetHtmlDocumentAsync(Uri uri)
		{
			var htmlDocument = new HtmlDocument();

			string html;
			using (var webClient = new WebClient())
			{
				webClient.Proxy = null;

				html = await webClient.DownloadStringTaskAsync(uri);
			}
			htmlDocument.LoadHtml(html);
			return htmlDocument;
		}

		

	}

	public class CssStylesheetUsageResult
	{
		public Uri Url { get; set; }

		public IEnumerable<CssSelector> UnusedCssSelectors { get; set; } 
	}
}
