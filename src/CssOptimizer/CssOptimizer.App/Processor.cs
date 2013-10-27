using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssOptimizer.Domain;
using CssOptimizer.Domain.Analysis;
using CssOptimizer.Domain.Utils;
using HtmlAgilityPack;

namespace CssOptimizer.App
{
	class Processor
	{
		private readonly CssStylesheets _cssStylesheets;

		public Processor()
		{
			_cssStylesheets = new CssStylesheets();
		}

		public async Task AnalyzeWebPages(Options options)
		{
			Cleanup(options);

			
			var tasks = options.Urls
				.Select(UrlHelper.CreateInvariantToScheme)
				.Select(uri => Task.Run(async () =>
				{

					var analyzer = new PageAnalyzer(_cssStylesheets);
					HtmlDocument htmlDocument = null;
					try
					{
						htmlDocument = await GetHtml(uri);
					}
					catch (UnsupportedContentTypeException ex)
					{
						return;
					}
					



					var analysisResult = await analyzer.Analyze(uri, htmlDocument);

					WriteResults(analysisResult, options);

				})).ToList();

			await Task.WhenAll(tasks);
		}

		public async Task AnalyzeWebSite(Options options)
		{
			Cleanup(options);

			var analyzedPages = new ConcurrentDictionary<string, Uri>();

			var pageUrl = UrlHelper.CreateInvariantToScheme(options.Urls[0]);

			analyzedPages.TryAdd(pageUrl.ToString(), pageUrl);
			
			await GetValue(pageUrl, analyzedPages, options);

		}

		private async Task GetValue(Uri pageUrl, ConcurrentDictionary<string, Uri> analyzedPages, Options options)
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
					if (analyzedPages.TryAdd(internalLink.ToString(), internalLink))
					{
						await GetValue(internalLink, analyzedPages, options);
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
			var formattedResults = FormatResults(pageAnalysisResult, options);

			Console.Write(formattedResults);

			if (!String.IsNullOrWhiteSpace(options.OutputFile))
				File.AppendAllText(options.GetOutputFileName(), formattedResults);
		}

		private string FormatResults(PageAnalysisResult pageAnalysisResult, Options options)
		{
			var sb = new StringBuilder();

			foreach (var cssUsageInfo in pageAnalysisResult.CssUsageInfos)
			{
				sb.AppendLine("=====================");
				sb.AppendFormat("{0} ---> {1}\r\n", pageAnalysisResult.Url, cssUsageInfo.Url);
				sb.AppendLine("=====================");


				if(!options.Quite)
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
