using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CssOptimizer.Domain
{
	public class CssStylesheet
	{
		private readonly List<string> _rules = new List<string>();

		public IEnumerable<string> Rules { get { return _rules; } }

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
			var styleName = CleanUp(parts[0]).Trim().ToLower();

			if (!_rules.Contains(styleName))
			{
				_rules.Add(styleName);
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
