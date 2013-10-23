using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CssOptimizer.Domain
{
	public class CssStylesheet
	{
		private readonly List<CssSelector> _selectors = new List<CssSelector>();

		public IEnumerable<CssSelector> Selectors { get { return _selectors; } }

		public CssStylesheet(string rawCss)
		{
			var css = CleanUp(rawCss);

			Process(css);
		}

		private void Process(string css)
		{
			var parts = css.Split('}');
			foreach (var s in parts)
			{
				if (CleanUp(s).IndexOf('{') > -1)
				{
					FillStyleClass(s);
				}
			}
		}

		private void FillStyleClass(string s)
		{
			var parts = s.Split('{');
			var selectors = CleanUp(parts[0]).Trim().ToLower().Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).Select(z => z.Trim()).ToList();

			foreach (var selector in selectors)
			{
				if (_selectors.All(z => z.OriginalSelector != selector))
				{
					_selectors.Add(new CssSelector(selector));
				}
			}
		}

		private string CleanUp(string s)
		{
			var copy = s;
			var regex = new Regex("(/\\*(.|[\r\n])*?\\*/)|(//.*)");
			copy = regex.Replace(copy, "");
			copy = copy.Replace("\r", "").Replace("\n", "");
			return copy;
		}
	}
}
