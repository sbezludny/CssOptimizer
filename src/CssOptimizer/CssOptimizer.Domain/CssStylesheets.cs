using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CssOptimizer.Domain
{
	public class CssStylesheets
	{
		private readonly ConcurrentDictionary<Uri, CssStylesheet> _stylesheets = new ConcurrentDictionary<Uri, CssStylesheet>();

		public async Task<CssStylesheet> GetOrDownload(Uri uri)
		{
			CssStylesheet stylesheet;

			if (_stylesheets.TryGetValue(uri, out stylesheet)) 
				return stylesheet;
			
			using (var webClient = new WebClient())
			{ 
				stylesheet = new CssStylesheet(uri, await webClient.DownloadStringTaskAsync(uri));
			}
			_stylesheets.TryAdd(uri, stylesheet);

			return stylesheet;
		}
	}
}
