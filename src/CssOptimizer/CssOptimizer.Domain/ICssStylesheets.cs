using System;

namespace CssOptimizer.Domain
{
	public interface ICssStylesheets
	{
		CssStylesheet GetOrDownload(Uri uri);
	}
}