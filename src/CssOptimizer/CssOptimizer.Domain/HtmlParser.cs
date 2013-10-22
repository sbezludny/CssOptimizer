using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CssOptimizer.Domain
{
	public class HtmlParser
	{
		public IEnumerable<Uri> GetExternalStylesheets(HtmlDocument html)
		{
			throw new NotImplementedException();
		}

		public string GetInlineCss(HtmlDocument html)
		{
			throw new NotImplementedException();
		}
	}
}
