using System;
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

		public async Task<Dictionary<Uri, IEnumerable<CssSelector>>> GetUnusedCssSelectors(Uri uri)
		{
			var result = new Dictionary<Uri, IEnumerable<CssSelector>>();

			var htmlDocument = GetHtml(uri);

			var styleSheets = new List<CssStylesheet>();

			var inlineCss = htmlDocument.GetInlineCss();

			if (!String.IsNullOrWhiteSpace(inlineCss))
			{
				styleSheets.Add(new CssStylesheet(uri, inlineCss));
			}

			var cssUrls = htmlDocument.GetExternalCssLinks().Select(href => UrlHelper.CreateFromHref(uri, href)).ToList();

			

			styleSheets.AddRange(await GetCssStylesheets(cssUrls));

			foreach (var stylesheet in styleSheets)
			{
				var unusedSelectors = GetUnusedSelectors(htmlDocument, stylesheet);

				if (unusedSelectors.Any())
				{
					result.Add(stylesheet.Url, unusedSelectors);
				}
			}

			return result;


		}

		

		private static HtmlDocument GetHtml(Uri uri)
		{
			var htmlDocument = new HtmlDocument();

			using (var webClient = new WebClient())
			{
				htmlDocument.LoadHtml(webClient.DownloadString(uri));
			}
			return htmlDocument;
		}

		private Task<CssStylesheet[]> GetCssStylesheets(IList<Uri> urls)
		{
			var tasks = new List<Task<CssStylesheet>>(urls.Count());
			
			tasks.AddRange(urls.Select(url => Task.Run(async () => await _stylesheets.GetOrDownload(url))));

			return Task.WhenAll(tasks);
		}

		private IEnumerable<CssSelector> GetUnusedSelectors(HtmlDocument document, CssStylesheet stylesheet)
		{
			return stylesheet.Selectors
				.Where(selector => !document.HasElementsWithSelector(selector)).ToList();
		} 


	}
}
