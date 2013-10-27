using System;
using System.Net;
using System.Threading.Tasks;

namespace CssOptimizer.Domain.Utils
{
	public static class WebClientHelper
	{
		public static async Task<string> DownloadStringAsync(Uri url)
		{
			string source;
			using (var webClient = new WebClient())
			{
				webClient.Proxy = null;

				try
				{
					source = await webClient.DownloadStringTaskAsync(url);
				}
				catch (WebException ex)
				{
					throw new ArgumentException(String.Format("Произошла ошибка при запросe страницы с адресом `{0}`.", url), ex);
				}

			}

			return source;
		}
	}
}