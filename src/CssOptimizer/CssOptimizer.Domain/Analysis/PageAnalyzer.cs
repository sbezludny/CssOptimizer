using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CssOptimizer.Domain.Utils;
using HtmlAgilityPack;

namespace CssOptimizer.Domain.Analysis
{
	public class PageAnalyzer
	{
		private readonly CssStylesheets _stylesheets;
		
		public PageAnalyzer(CssStylesheets stylesheets)
		{
			_stylesheets = stylesheets;
		}

		public async Task<PageAnalysisResult> Analyze(Uri pageUrl, HtmlDocument html)
		{
			var cssInfos = new List<CssUsageInfo>();

			

			if (!String.IsNullOrWhiteSpace(html.GetInlineStyles()))
			{
				cssInfos.Add(AnalyzeInternalStyles(pageUrl, html)); 
			}

			cssInfos.AddRange(await AnalyzeExternalStyles(pageUrl, html));

			var filteredCssInfos = cssInfos.Where(z => z.UnusedSelectors.Any()).ToList();
			
			return new PageAnalysisResult(pageUrl, filteredCssInfos)
			{
				InternalLinks = html.GetInternalLinks(pageUrl)
			};
		}

		private async Task<CssUsageInfo[]> AnalyzeExternalStyles(Uri pageUrl, HtmlDocument html)
		{
			var cssUrls = html
				.GetExternalCssUrls()
				.Select(href => UrlHelper.CreateFromHref(pageUrl, href))
				.ToList();



			var tasks = cssUrls.Select(cssUrl => Task.Run(async () =>
			{
				var stylesheet = await _stylesheets.GetOrDownload(cssUrl);

				return AnalyzeCssStylesheet(stylesheet, html);

			})).ToList();

			return await Task.WhenAll(tasks);

		}

		private static CssUsageInfo AnalyzeInternalStyles(Uri pageUrl, HtmlDocument html)
		{
			var inlineCss = html.GetInlineStyles();

			var styleSheet = new CssStylesheet(pageUrl, inlineCss);

			return AnalyzeCssStylesheet(styleSheet, html);
		}

		

		private static CssUsageInfo AnalyzeCssStylesheet(CssStylesheet stylesheet, HtmlDocument html)
		{
			var unusedSelectors = stylesheet.Selectors
				.AsParallel()
				.Where(s => !html.IsSelectorInUse(s))
				.ToList();
			
			return new CssUsageInfo(stylesheet.Url, unusedSelectors);
		}

		

	}
}
