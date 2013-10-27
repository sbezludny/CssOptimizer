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

			string css;

			using (var webClient = new WebClient())
			{
				webClient.Proxy = null;
				css = await webClient.DownloadStringTaskAsync(uri);
			}

			stylesheet = new CssStylesheet(uri, css);
			
			_stylesheets.TryAdd(uri, stylesheet);

			return stylesheet;
		}
	}
}
