using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CssOptimizer.Domain
{
	public class CssStylesheets : ICssStylesheets
	{
		private readonly ConcurrentDictionary<Uri, CssStylesheet> _stylesheets = new ConcurrentDictionary<Uri, CssStylesheet>(); 

		public CssStylesheet GetOrDownload(Uri uri)
		{
			CssStylesheet stylesheet;

			if (!_stylesheets.TryGetValue(uri, out stylesheet))
			{
				using (var webClient = new WebClient())
				{
					var cssContent = webClient.DownloadString(uri);

					stylesheet = new CssStylesheet(uri, cssContent);
				}
				_stylesheets.TryAdd(uri, stylesheet);
			}

			return stylesheet;
		}
	}
}
