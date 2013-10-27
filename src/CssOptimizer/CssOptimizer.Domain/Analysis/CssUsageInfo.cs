using System;
using System.Collections.Generic;

namespace CssOptimizer.Domain.Analysis
{
	public class CssUsageInfo
	{
		public Uri Url { get; set; }
		public IEnumerable<CssSelector> UnusedSelectors { get; set; }

		public CssUsageInfo(Uri url, IEnumerable<CssSelector> unusedSelectors)
		{
			Url = url;
			UnusedSelectors = unusedSelectors;
		}
	}
}