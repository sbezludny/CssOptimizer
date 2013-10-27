using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CssOptimizer.Domain;
using NUnit.Framework;

namespace CssOptimizer.Tests
{
	[TestFixture]
	public class CssStylesheetTests
	{
		[TestCaseSource("CssDataSource")]
		public void SelectorsExtraction(string css, IEnumerable<string> expectedSelectors)
		{
			//Arrange

			//Act
			var stylesheet = new CssStylesheet(null, css );

			//Assert
			CollectionAssert.AreEquivalent(expectedSelectors, stylesheet.Selectors.Select(z => z.RawSelector));

		}

		[TestCaseSource("ImportsDataSource")]
		public void ImportUrlsExtractions(string css, IEnumerable<string> importUrls)
		{
			//Arrange

			//Act
			var styleSheet = new CssStylesheet(null, css);

			//Assert
			CollectionAssert.AreEquivalent(importUrls,styleSheet.Imports);
		}

		public static IEnumerable CssDataSource
		{
			get
			{
				yield return new TestCaseData("li { }", new[] { "li" });
				yield return new TestCaseData("li li.selected { }", new[] { "li li.selected" });
				yield return new TestCaseData("li, li.selected { }", new[] { "li", "li.selected" });
				yield return new TestCaseData("li {} li.selected { }", new[] { "li", "li.selected" });
				yield return new TestCaseData(@"@import url(""fineprint.css"") print; li {} li.selected { }", new[] { "li", "li.selected" });

			}
		}

		public static IEnumerable ImportsDataSource
		{
			get
			{
				yield return new TestCaseData("li { }", new string[] { });	
				yield return new TestCaseData(@"@import url(""fineprint.css"") print;
												@import url(""bluish.css"") projection, tv;
												@import 'custom.css';
												@import url(""chrome://communicator/skin/"");
												@import ""common.css"" screen, projection;
												@import url('landscape.css') screen and (orientation:landscape);", 
												new []
												{
													"fineprint.css",
													"bluish.css",
													"custom.css",
													"chrome://communicator/skin/",
													"common.css",
													"landscape.css",
												});	
			}
		} 
	}
}
