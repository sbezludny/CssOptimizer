using System;
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


	public class CssSelector
	{
		

		/// <summary>
		/// Селектор тега
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// Классы
		/// </summary>
		public List<string> Classes { get; set; }

		/// <summary>
		/// Идентификатор
		/// </summary>
		public string Identifier { get; set; }

		/// <summary>
		/// Контекстные селекторы
		/// </summary>
		public List<CssSelector> ContextSelectors { get; set; }

		/// <summary>
		/// Соседние селекторы
		/// Селектор 1 + Селектор 2
		/// </summary>
		public List<CssSelector> TraversalSelectors { get; set; }


		/// <summary>
		/// Дочерние селекторы
		/// Селектор 1 > Селектор 2
		/// </summary>
		public IEnumerable<CssSelector> ChildSelectors { get; set; }

		/// <summary>
		/// Селекторы атрибутов
		/// </summary>
		public List<string> Attributes { get; set; }

		public string PseudoClass { get; set; }

		public string PseudoElement { get; set; }

		public string UniversalSelector { get; set; }

		internal CssSelector(){}

		public CssSelector(string selector)
		{

		}
	}

	public class CssAttribute
	{
		public string Key { get; set; }
		public string Value { get; set; }

		public CssAttributeMathcingRule MathingRule { get; set; }
	}

	public enum CssAttributeMathcingRule
	{
		/// <summary>
		/// [title]
		/// </summary>
		Exists = 0,
		/// <summary>
		/// [title=a]
		/// </summary>
		Equals = 1,
		/// <summary>
		/// [title~=a]
		/// </summary>
		ContainsWord = 2,
		/// <summary>
		/// [title|=a]
		/// </summary>
		ContainsPrefix = 3
	}
}
