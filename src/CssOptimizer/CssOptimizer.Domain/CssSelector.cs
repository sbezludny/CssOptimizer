using System;
using System.Linq;
using CssOptimizer.Domain.Utils;

namespace CssOptimizer.Domain
{
	public class CssSelector
	{
		public string RawSelector { get; set; }
		
		public static readonly string[] UnsupportedAtRules =
		{
			"@charset",
			"@document",
			"@font-face",
			"@import",
			"@keyframes",
			"@media",
			"@namespace",
			"@page",
			"@supports"
		};

		public static readonly string[] UnsupportedPseudoSelectors =
		{
			":indeterminate", 
			":first-line", 
			":first-letter",
            ":selection", 
			":before", 
			":after", 
			":link", 
			":visited",
			":active", 
			":focus", 
			":hover",
			":enabled"
		};
		
		public CssSelector(string selector)
		{
			Ensure.NotNullOrEmpty(selector, "selector");

			RawSelector = selector;
		}
		public string ToXPath()
		{
			if (UnsupportedPseudoSelectors.Any(RawSelector.Contains))
			{
				var message = String.Format("Селектор `{0}` содержит не поддерживаемый селектор `{1}`.",
									RawSelector,
									UnsupportedPseudoSelectors.FirstOrDefault(RawSelector.Contains));
				throw new UnsupportedSelectorException(message);
			}

			if (UnsupportedAtRules.Any(RawSelector.Contains))
			{
				throw new UnsupportedSelectorException("@-правила не поддерживаются.");
			}

			return CssSelectorParser.Transform(RawSelector);
		}

		public override string ToString()
		{
			return RawSelector;
		}

		
	}

	public class UnsupportedSelectorException : Exception
	{
		public UnsupportedSelectorException(string message)
			: base(message)
		{
		}
	}
}