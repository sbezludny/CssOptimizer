using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CssOptimizer.Domain
{
	public static class CssSelectorParser
	{
		private static readonly Dictionary<Regex, dynamic> Patterns = new Dictionary<Regex, dynamic>();

		static CssSelectorParser()
		{
			// Селекторы атрибутов
			Patterns.Add(new Regex(@"\[([^\]~\$\*\^\|\!]+)(=[^\]]+)?\]"), "[@$1$2]");

			// Убираем пробелы вокруг +, ~, and >
			Patterns.Add(new Regex(@"\s*(\+|~|>)\s*"), "$1");

			//*, ~, +, and >
			Patterns.Add(new Regex(@"([a-zA-Z0-9_\-\*])~([a-zA-Z0-9_\-\*])"), "$1/following-sibling::$2");
			Patterns.Add(new Regex(@"([a-zA-Z0-9_\-\*])\+([a-zA-Z0-9_\-\*])"), "$1/following-sibling::*[1]/self::$2");
			Patterns.Add(new Regex(@"([a-zA-Z0-9_\-\*])>([a-zA-Z0-9_\-\*])"), "$1/$2");

			// Escaping
			Patterns.Add(new Regex(@"\[([^=]+)=([^'|" + "\"" + @"][^\]]*)\]"), "[$1='$2']");

			Patterns.Add(new Regex(@"(^|[^a-zA-Z0-9_\-\*])(#|\.)([a-zA-Z0-9_\-]+)"), "$1*$2$3");
			Patterns.Add(new Regex(@"([\>\+\|\~\,\s])([a-zA-Z\*]+)"), "$1//$2");
			Patterns.Add(new Regex(@"\s+\/\/"), "//");

			// :first-child
			Patterns.Add(new Regex(@"([a-zA-Z0-9_\-\*\.\#]+):first-child"), "*[1]/self::$1");
			
			// :last-child
			Patterns.Add(new Regex(@"([a-zA-Z0-9_\-\*]+):last-child"), "$1[not(following-sibling::*)]");

			// :only-child
			Patterns.Add(new Regex(@"([a-zA-Z0-9_\-\*]+):only-child"), "*[last()=1]/self::$1");

			// :empty
			Patterns.Add(new Regex(@"([a-zA-Z0-9_\-\*]+):empty"), "$1[not(*) and not(normalize-space())]");

			// :not
			Patterns.Add(new Regex(@"([a-zA-Z0-9_\-\*]+):not\(([^\)]*)\)"), new MatchEvaluator(m => m.Groups[1].Value + "[not(" + (new Regex("^[^\\[]+\\[([^\\]]*)\\].*$")).Replace(Transform(m.Groups[2].Value), "$1") + ")]"));

			// :nth-child
			Patterns.Add(new Regex(@"([a-zA-Z0-9_\-\*]+):nth-child\(([^\)]*)\)"), new MatchEvaluator(m =>
			{
				var b = m.Groups[2].Value;
				var a = m.Groups[1].Value;

				switch (b)
				{
					case "n":
						return a;
					case "even":
						return "*[position() mod 2=0 and position()>=0]/self::" + a;
					case "odd":
						return a + "[(count(preceding-sibling::*) + 1) mod 2=1]";
					default:
						b = ((new Regex("^([0-9])*n.*?([0-9])*$")).Replace(b, "$1+$2"));

						var b2 = new String[2];
						var splitResult = b.Split('+');

						b2[0] = splitResult[0];

						var buffer = 0;

						if (splitResult.Length == 2)
						{
							if (!int.TryParse(splitResult[1], out buffer))
							{
								buffer = 0;
							}}

						b2[1] = buffer.ToString();

						return "*[(position()-" + b2[1] + ") mod " + b2[0] + "=0 and position()>=" + b2[1] + "]/self::" + a;
				}
			}));

			// :contains
			Patterns.Add(new Regex(@":contains\(([^\)]*)\)"), new MatchEvaluator(m => "[contains(string(.),'" + m.Groups[1].Value + "')]"));

			// Псевдокласс :disabled
			Patterns.Add(new Regex(@"([a-zA-Z0-9_\-\*]+):disabled"), "$1[@disabled]");

			// Псевдокласс :checked
			Patterns.Add(new Regex(@"([a-zA-Z0-9_\-\*]+):checked"), "$1[@checked]");

			// != атрибут
			Patterns.Add(new Regex(@"\[([a-zA-Z0-9_\-]+)\|=([^\]]+)\]"), "[@$1=$2 or starts-with(@$1,concat($2,'-'))]");

			// *= атрибут
			Patterns.Add(new Regex(@"\[([a-zA-Z0-9_\-]+)\*=([^\]]+)\]"), "[contains(@$1,$2)]");

			// ~= атрибут
			Patterns.Add(new Regex(@"\[([a-zA-Z0-9_\-]+)~=([^\]]+)\]"), "[contains(concat(' ', normalize-space(@$1), ' '), concat(' ',$2,' '))]");

			// ^= атрибут
			Patterns.Add(new Regex(@"\[([a-zA-Z0-9_\-]+)\^=([^\]]+)\]"), "[starts-with(@$1,$2)]");

			// $= атрибут
			Patterns.Add(new Regex(@"\[([a-zA-Z0-9_\-]+)\$=([^\]]+)\]"), new MatchEvaluator(m =>
			{
				var a = m.Groups[1].Value;
				var b = m.Groups[2].Value;
				return "[substring(@" + a + ",string-length(@" + a + ")-" + (b.Length - 3) + ")=" + b + "]";
			}));

			// != атрибут
			Patterns.Add(new Regex(@"\[([a-zA-Z0-9_\-]+)\!=([^\]]+)\]"), "[not(@$1) or @$1!=$2]");

			// Селекторы идентификаторов
			Patterns.Add(new Regex(@"#([a-zA-Z0-9_\-]+)"), "[@id='$1']");
			
			// Селекторы классов
			Patterns.Add(new Regex(@"\.([a-zA-Z0-9_\-]+)"), "[contains(concat(' ', normalize-space(@class), ' '), ' $1 ')]");

			// cleanup xpath
			Patterns.Add(new Regex(@"\]\[([^\]]+)"), " and ($1)");

			
		}


		public static string Transform(string css)
		{
			var copy = css;
			foreach (var pattern in Patterns)
			{
				copy = pattern.Key.Replace(copy, pattern.Value);
			}

			return "//" + copy;
		}
	}
}