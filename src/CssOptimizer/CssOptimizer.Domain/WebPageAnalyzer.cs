using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CssOptimizer.Domain
{
	public class WebPageAnalyzer
	{


		public Dictionary<Uri, IEnumerable<string>> GetUnusedCssSelectors(Uri uri)
		{
			throw new NotImplementedException();
		}

		private string Download(Uri uri)
		{
			var tmpPath = Path.GetTempFileName();

			using (var webClient = new WebClient())
			{
				webClient.DownloadFile(uri, tmpPath);
			}
			return tmpPath;
		}
	}
}
