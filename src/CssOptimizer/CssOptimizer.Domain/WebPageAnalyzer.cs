using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CssOptimizer.Domain
{
	public class WebPageAnalyzer
	{
		private readonly ICssStylesheets _stylesheets;

		public WebPageAnalyzer(ICssStylesheets stylesheets)
		{
			_stylesheets = stylesheets;
		}

		public Dictionary<Uri, IEnumerable<CssSelector>> GetUnusedCssSelectors(Uri uri)
		{
			var result = new Dictionary<Uri, IEnumerable<CssSelector>>();

			var htmlDocument = new HtmlDocument();

			using (var webClient = new WebClient())
			{
				htmlDocument.LoadHtml(webClient.DownloadString(uri));
			}

			var cssUris = htmlDocument.GetExternalCssLinks().Select(href => ConvertToUri(uri, href));

			var styleSheets = cssUris.Select(cssUri => _stylesheets.GetOrDownload(cssUri)).ToList();

			var inlineCss = htmlDocument.GetInlineCss();

			if (!String.IsNullOrWhiteSpace(inlineCss))
			{
				styleSheets.Add(new CssStylesheet(uri, inlineCss));
			}

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

		private IEnumerable<CssSelector> GetUnusedSelectors(HtmlDocument document, CssStylesheet stylesheet)
		{
			return stylesheet.Selectors
				.Where(selector => !document.HasElementsWithSelector(selector))
				.ToList();
		} 

		private Uri ConvertToUri(Uri baseUrl, string href)
		{
			if(Uri.IsWellFormedUriString(href, UriKind.Absolute))
				return new Uri(href);

			return new Uri(baseUrl, href);
		}

	}
}
