using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssOptimizer.Domain;
using CssOptimizer.Domain.Analysis;
using CssOptimizer.Domain.Exceptions;
using CssOptimizer.Domain.Utils;
using HtmlAgilityPack;

namespace CssOptimizer.App
{
	class Processor
	{
		private readonly CssStylesheets _cssStylesheets = new CssStylesheets();

		public async Task AnalyzeWebPages(Options options)
		{
			Cleanup(options);

			
			var tasks = options.Urls
				.Select(UrlHelper.CreateInvariantToScheme)
				.Select(uri => Task.Run(async () =>
				{

					var analyzer = new PageAnalyzer(_cssStylesheets);
					try
					{
						var htmlDocument = await GetHtml(uri);

						var analysisResult = await analyzer.Analyze(uri, htmlDocument);

						WriteResults(analysisResult, options);
					}
					catch (UnsupportedContentTypeException ex)
					{
						
					}

				})).ToList();

			await Task.WhenAll(tasks);
		}

		public async Task AnalyzeWebSites(Options options)
		{
			Cleanup(options);

			var analyzedPages = new ConcurrentDictionary<string, Uri>();

			var pageUrl = UrlHelper.CreateInvariantToScheme(options.Urls[0]);

			analyzedPages.TryAdd(pageUrl.ToString(), pageUrl);
			
			await AnalyzePage(pageUrl, analyzedPages, options);

		}

		private async Task AnalyzePage(Uri pageUrl, ConcurrentDictionary<string, Uri> analyzedPages, Options options)
		{

			PageAnalysisResult analysisResult;

			try
			{
				analysisResult = await GetPageAnalysis(pageUrl);
			}
			catch (UnsupportedSelectorException ex)
			{
				return;
			}
			

			WriteResults(analysisResult, options);

			foreach (var internalLink in analysisResult.InternalLinks)
			{
				if (analyzedPages.Count < options.MaximumPages || options.MaximumPages == 0)
				{
					if (options.MaximumDepth != 0)
					{
						if (internalLink.Segments.Length > options.MaximumDepth)
							return;
					}
					
					if (analyzedPages.TryAdd(internalLink.ToString(), internalLink))
					{
						await AnalyzePage(internalLink, analyzedPages, options);
					}
				}

				
			}
		}

		private async Task<PageAnalysisResult> GetPageAnalysis(Uri pageUrl)
		{
			var analyzer = new PageAnalyzer(_cssStylesheets);

			var htmlDocument = await GetHtml(pageUrl);
			var analysisResult = await analyzer.Analyze(pageUrl, htmlDocument);
			return analysisResult;
		}

		private void Cleanup(Options options)
		{
			if (!String.IsNullOrWhiteSpace(options.OutputFile))
			{
				File.Create(options.GetOutputFileName());
			}
		}

		private void WriteResults(PageAnalysisResult pageAnalysisResult, Options options)
		{
			var formattedResults = FormatResults(pageAnalysisResult);

			Console.Write(formattedResults);

			if (!String.IsNullOrWhiteSpace(options.OutputFile))
				File.AppendAllText(options.GetOutputFileName(), formattedResults);
		}

		private string FormatResults(PageAnalysisResult pageAnalysisResult)
		{
			var sb = new StringBuilder();

			foreach (var cssUsageInfo in pageAnalysisResult.CssUsageInfos)
			{
				sb.AppendLine("=====================");
				sb.AppendFormat("{0} ---> {1}\r\n", pageAnalysisResult.Url, cssUsageInfo.Url);
				sb.AppendFormat("Количество неиспользуемых селекторов: {0}\r\n", cssUsageInfo.UnusedSelectors.Count());
				sb.AppendLine("=====================");


				cssUsageInfo.UnusedSelectors.ToList().ForEach((selector => sb.AppendLine(selector.ToString())));

				
			}

			return sb.ToString();
		}

		private async Task<HtmlDocument> GetHtml(Uri url)
		{
			var html = await WebClientHelper.DownloadStringAsyncStrict(url);

			var htmlDocument = new HtmlDocument();

			htmlDocument.LoadHtml(html);

			return htmlDocument;
		}
	}
}
