using System;
using System.Collections.Generic;

namespace CssOptimizer.Domain.Analysis
{
	public class PageAnalysisResult
	{
		private IEnumerable<Uri> _internalLinks = new List<Uri>();
		public Uri Url { get; set; }
		public IEnumerable<CssUsageInfo> CssUsageInfos { get; set; }

		public IEnumerable<Uri> InternalLinks
		{
			get { return _internalLinks; }
			set { _internalLinks = value; }
		}


		public PageAnalysisResult(Uri url, IEnumerable<CssUsageInfo> cssUsageInfos)
		{
			Url = url;
			CssUsageInfos = cssUsageInfos;
		}
	}
}