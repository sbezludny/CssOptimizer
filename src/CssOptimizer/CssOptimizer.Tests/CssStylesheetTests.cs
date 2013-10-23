using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssOptimizer.Domain;
using NUnit.Framework;
using ServiceStack.Text;

namespace CssOptimizer.Tests
{
	[TestFixture]
	public class CssStylesheetTests
	{
		[TestCaseSource("CssDataSource")]
		public void ConstructorTests(string css, IEnumerable<string> expectedSelectors)
		{
			//Arrange

			//Act
			var stylesheet = new CssStylesheet(css);

			//Assert
			Assert.AreEqual(expectedSelectors.ToJson(), stylesheet.Selectors.Select(z => z.OriginalSelector).ToJson());

		}

		public static IEnumerable CssDataSource
		{
			get
			{
				yield return new TestCaseData("li { }", new[] { "li" });
				yield return new TestCaseData("li li.selected { }", new[] { "li li.selected" });
				yield return new TestCaseData("li, li.selected { }", new[] { "li", "li.selected" });
				yield return new TestCaseData("li {} li.selected { }", new[] { "li", "li.selected" });

			}
		}
	}
}
