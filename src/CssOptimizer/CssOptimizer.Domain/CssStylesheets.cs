using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using CssOptimizer.Domain.Utils;

namespace CssOptimizer.Domain
{
	/// <summary>
	/// Stylesheet repository.
	/// </summary>
	public class CssStylesheets
	{
		private readonly ConcurrentDictionary<Uri, CssStylesheet> _stylesheets = new ConcurrentDictionary<Uri, CssStylesheet>();

		public async Task<CssStylesheet> GetOrDownload(Uri url)
		{
			CssStylesheet stylesheet;

			if (_stylesheets.TryGetValue(url, out stylesheet)) 
				return stylesheet;

			string css = await WebClientHelper.DownloadStringAsync(url);

			stylesheet = new CssStylesheet(url, css);
			
			_stylesheets.TryAdd(url, stylesheet);

			return stylesheet;
		}
	}
}
