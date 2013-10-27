using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssOptimizer.Domain.Utils
{
	public static class UrlHelper
	{
		public static Uri CreateFromHref(Uri baseUri, string relativeOrAbsolute)
		{
			if (Uri.IsWellFormedUriString(relativeOrAbsolute, UriKind.Absolute))
				return new Uri(relativeOrAbsolute);

			return new Uri(baseUri, relativeOrAbsolute);
		}

		public static Uri CreateInvariantToScheme(string urlString)
		{
			var url = urlString;
			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				url = "http://" + url;
			}
			return new Uri(url);
		}
	}
}
