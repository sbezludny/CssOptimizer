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

			var htmlSource = await WebClientHelper.DownloadStringAsync(pageUrl);
			
			var html = ParseHtml(htmlSource);


			var cssUrls = html.GetExternalCssLinks()
				.Select(href => UrlHelper.CreateFromHref(pageUrl, href)).ToList();

			var tasks = cssUrls.Select(cssUrl => Task.Run(async () =>
			{
				CssStylesheet stylesheet;
				try
				{
					stylesheet = await _stylesheets.GetOrDownload(cssUrl);
				}
				catch (Exception ex)
				{
					throw new ArgumentException(String.Format("Произошла ошибка при запросe css-файла с адресом `{0}`.", pageUrl), ex);
				}
				

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
