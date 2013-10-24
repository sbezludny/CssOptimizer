using System;
using System.Text.RegularExpressions;

namespace CssOptimizer.Domain
{
	public class CssAttribute
	{
		public static string MatchingPattern = @"\[(?<name>\w+)\s*(?<operator>[^\w,""[]{0,2})\s*[""|']*(?<value>[a-zA-Z0-9_ ]*)?\s*[""|']*[,|\]]";

		public string Name { get; set; }
		public string Value { get; set; }

		public CssAttributeOperator Operator { get; set; }

		internal CssAttribute()
		{
			
		}

		public string ToXPath()
		{
			string selector;

			switch (Operator)
			{
				case CssAttributeOperator.Exists:
					selector = "@{0}";
					break;
				case CssAttributeOperator.Equals:
					selector = "@{0}='{1}'";
					break;
				case CssAttributeOperator.ContainsWord:
					selector = "contains(concat(' ', normalize-space(@{0}), ' '), ' {1} ')";
					break;
				case CssAttributeOperator.ContainsPrefix:
					selector = "@name = '{1}' or starts-with(@{0}, '{1}-')";
					break;
				case CssAttributeOperator.StartsWith:
					selector = "starts-with(@{0}, '{1}')";
					break;
				case CssAttributeOperator.EndsWith:
					selector = "substring(@{0}, string-length(@{0})-4) = '{1}'";
					break;
				case CssAttributeOperator.Contains:
					selector = "contains(@{0}, '{1}')";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return String.Format(selector, Name, Value);
		}

		public CssAttribute(string selector)
		{
			//todo:refactor

			var regex = new Regex(MatchingPattern);

			var matches = regex.Matches(selector);

			foreach (Match match in matches)
			{
				Name = match.Groups["name"].Value;

				var operatorGroup = match.Groups["operator"];

				if (operatorGroup.Success)
				{
					Operator = CssAttributeOperatorExtensions.FromString(operatorGroup.Value);
				}

				var valueGroup = match.Groups["value"];

				if (!String.IsNullOrWhiteSpace(valueGroup.Value))
				{
					Value = valueGroup.Value;
				}
				/*ProcessId(match.Groups["Id"].Value);

				ProcessClasses(match.Groups["Classes"].Value);*/

			}
		}
	}
}