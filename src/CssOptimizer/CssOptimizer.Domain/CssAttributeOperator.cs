using System;

namespace CssOptimizer.Domain
{
	public enum CssAttributeOperator
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
		ContainsPrefix = 3,
		/// <summary>
		/// [title^=a]
		/// </summary>
		BeginsWith = 4,
		/// <summary>
		/// [title$=a]
		/// </summary>
		EndsWith = 5,
		/// <summary>
		/// [title*=a]
		/// </summary>
		Contains = 6
	}

	public static class CssAttributeOperatorExtensions
	{
		public static CssAttributeOperator FromString(string operatorStr)
		{
			CssAttributeOperator attributeOperator;

			switch (operatorStr)
			{
				case "":
					attributeOperator = CssAttributeOperator.Exists;
					break;
				case "=":
					attributeOperator = CssAttributeOperator.Equals;
					break;
				case "~=":
					attributeOperator = CssAttributeOperator.ContainsWord;
					break;
				case "|=":
					attributeOperator = CssAttributeOperator.ContainsPrefix;
					break;
				case "^=":
					attributeOperator = CssAttributeOperator.BeginsWith;
					break;
				case "$=":
					attributeOperator = CssAttributeOperator.EndsWith;
					break;
				case "*=":
					attributeOperator = CssAttributeOperator.Contains;
					break;
				default:
					throw new ArgumentOutOfRangeException("operatorStr");
			}

			return attributeOperator;
		}
	}
}