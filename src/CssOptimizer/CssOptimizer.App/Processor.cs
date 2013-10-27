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

namespace CssOptimizer.App
{
	class Processor
	{
		private readonly CssStylesheets _cssStylesheets;

		public Processor(CssStylesheets cssStylesheets)
		{
			_cssStylesheets = cssStylesheets;
		}

		public async Task AnalyzeWebPages(Options options)
		{
			Cleanup(options);

			var tasks = options.Urls
				.Select(UrlHelper.CreateInvariantToScheme)
				.Select(uri => Task.Run(async () =>
				{

					var analyzer = new PageAnalyzer(_cssStylesheets);

					var analysisResult = await analyzer.Analyze(uri);

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
			var analysisResult = await GetPageAnalysis(pageUrl);

			WriteResults(analysisResult, options);

			foreach (var internalLink in analysisResult.InternalLinks)
			{
				if (analyzedPages.Count < options.MaximumPages)
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

			var analysisResult = await analyzer.Analyze(pageUrl);
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
				sb.AppendLine("=====================");

				foreach (var selector in cssUsageInfo.UnusedSelectors)
				{
					sb.AppendLine(selector.ToString());
				}
			}

			return sb.ToString();
		}
	}
}
