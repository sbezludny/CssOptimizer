using System;
using System.Linq;
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

		public static async Task<string> DownloadStringAsyncStrict(Uri url)
		{
			if (!await IsUrlContainsText(url))
				throw new UnsupportedContentTypeException(String.Format("Формат файла {0} не поддерживается.", url));
			return await DownloadStringAsync(url);

		}

		public static async Task<bool> IsUrlContainsText(Uri uri)
		{
			string[] availableContentTypes = { "text/html", "text/css" };

			var webRequest = WebRequest.CreateHttp(uri);
			webRequest.Timeout = 15000;
			webRequest.Method = "HEAD";

			bool result;

			using (var response = await webRequest.GetResponseAsync())
			{
				var contentType = response.ContentType.Split(';')[0].ToLower();
				result = availableContentTypes.Contains(contentType);
			}

			return result;
		}



		
	}

}