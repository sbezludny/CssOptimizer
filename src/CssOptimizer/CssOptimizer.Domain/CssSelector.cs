using System;
using System.Linq;
using System.Text.RegularExpressions;
using CssOptimizer.Domain.Exceptions;
using CssOptimizer.Domain.Utils;

namespace CssOptimizer.Domain
{
	public class CssSelector
	{
		public string RawSelector { get; set; }

		private static readonly string[] SupportedPseudoClasses =
		{
			":first-child", 
			":last-child", 
			":only-child", 
			":nth-child", 
			":empty", 
			":not", 
			":contains", 
			":disabled", 
			":checked"
		};

		private readonly string _supportedPseudoClassesPattern = String.Join("|", SupportedPseudoClasses);

		public CssSelector(string selector)
		{
			Ensure.NotNullOrEmpty(selector, "selector");

			EnsureSupportedPseudoClass(selector);

			if (selector.StartsWith("@"))
				throw new UnsupportedSelectorException("@-правила не поддерживаются.");

			if (selector.Contains("%"))
				throw new UnsupportedSelectorException("@keyframe не поддерживается.");

			RawSelector = selector;
		}

		private void EnsureSupportedPseudoClass(string selector)
		{
			var s = selector;
			if (s.Contains(":"))
			{	
				if (Regex.Replace(s, _supportedPseudoClassesPattern, "").Contains(":"))
				{
					var message = String.Format("Селектор `{0}` содержит не поддерживаемый псевдокласс.", selector);
					throw new UnsupportedSelectorException(message);
				}
			}
		}

		public string ToXPath()
		{
			return CssSelectorParser.Transform(RawSelector);
		}

		public override string ToString()
		{
			return RawSelector;
		}


	}
}