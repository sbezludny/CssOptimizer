using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CssOptimizer.Domain
{
	public class CssSelector
	{
		public string OriginalSelector { get; set; }

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
		public string Id { get; set; }

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
		public List<CssAttribute> Attributes { get; set; }

		public List<string> PseudoClasses { get; set; }

		public string PseudoElement { get; set; }

		public string UniversalSelector { get; set; }

		internal CssSelector()
		{
			Classes = new List<string>();
			PseudoClasses = new List<string>();
			ContextSelectors = new List<CssSelector>();
			TraversalSelectors = new List<CssSelector>();
			ChildSelectors = new List<CssSelector>();
			Attributes = new List<CssAttribute>();
		}

		public CssSelector(string selector):this()
		{
			OriginalSelector = selector;
			const string pattern =
				@"^(?<Type>[\*|\w|\-]+)?(?<Id>#[\w|\-]+)?(?<Classes>\.[\w|\-|\.]+)*(?<Attributes>\[.+\])*(?<PseudoClasses>:[\*|\w]+)*$";

			var regex = new Regex(pattern);

			var matches = regex.Matches(selector);

			foreach (Match match in matches)
			{

				//todo:refactor
				

				ProcessType(match.Groups["Type"].Value);

				ProcessId(match.Groups["Id"].Value);

				ProcessClasses(match.Groups["Classes"].Value);

				ProcessAttributes(match.Groups["Attributes"].Value);
			}

		}

		private void ProcessClasses(string selector)
		{
			if (String.IsNullOrWhiteSpace(selector))
				return;

			var regex = new Regex(@".(?<class>\w+)");
			var matchResult = regex.Match(selector);
			while (matchResult.Success)
			{
				Classes.Add(matchResult.Groups["class"].Value);
				matchResult = matchResult.NextMatch();
			} 

		}

		private void ProcessAttributes(string selector)
		{
			if (String.IsNullOrWhiteSpace(selector))
				return;

			var regex = new Regex(CssAttribute.MatchingPattern);
			var matchResult = regex.Match(selector);
			while (matchResult.Success)
			{
				Attributes.Add(new CssAttribute(matchResult.Value));
				matchResult = matchResult.NextMatch();
			} 


		}

		private void ProcessId(string id)
		{
			if (String.IsNullOrWhiteSpace(id))
				return;

			Id = id.TrimStart('#');
		}

		private void ProcessType(string type)
		{
			if (String.IsNullOrWhiteSpace(type))
				return;

			if (type == "*")
			{
				UniversalSelector = type;
			}
			else
			{
				Tag = type;
			}
		}

		public override string ToString()
		{
			return OriginalSelector;
		}

		public string ToXPath()
		{
			var xpath = "//";

			if (UniversalSelector != null)
				xpath += "*";

			if (Tag != null)
				xpath += Tag;

			if (Attributes.Any())
			{
				xpath += String.Format("[{0}]", String.Join(" and ", Attributes.Select(a => a.ToXPath())));
			}

			//todo:remove
			if (xpath == "//")
				xpath += "*";

			return xpath;
		}
	}
}