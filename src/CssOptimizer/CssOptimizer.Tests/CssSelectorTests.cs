using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CssOptimizer.Domain;
using NUnit.Framework;

namespace CssOptimizer.Tests
{
	[TestFixture]
	public class CssSelectorTests
	{
		[TestCaseSource("SelectorsDataSource", Category = "Common")]
		[TestCaseSource("AttributesDataSource", Category = "Attributes")]
		[TestCaseSource("ClassDataSource", Category = "Class selectors")]
		public void ConstructorTests(string selector, object expected)
		{

			//Arrange - Act
			var cssSelector = new CssSelector(selector);

			//Assert
			AssertEx.PropertyValuesAreEquals(cssSelector, expected);
		}


		/// <summary>
		/// Полный список рекомендованных тестов
		/// http://www.w3.org/Style/CSS/Test/CSS3/Selectors/current/html/full/flat/index.html
		/// </summary>
		public static IEnumerable SelectorsDataSource
		{
			get
			{
				yield return new TestCaseData("li", new CssSelector { Tag = "li" });
				yield return new TestCaseData("*", new CssSelector { UniversalSelector = "*" });
				yield return new TestCaseData("#foo", new CssSelector { Id = "foo" });
				yield return new TestCaseData("li#t2", new CssSelector { Tag = "li", Id = "t2" });
				

			}
		}

		public static IEnumerable ClassDataSource
		{
			get
			{
				yield return new TestCaseData(".t1", new CssSelector { Classes = {"t1"}});
				yield return new TestCaseData("li.t2", new CssSelector { Tag = "li", Classes = {"t2"}});
				yield return new TestCaseData("div.te.st", new CssSelector { Tag = "div", Classes = {"te", "st"}});
				yield return new TestCaseData(".te.st", new CssSelector { Classes = {"te", "st"}});
				yield return new TestCaseData(".t1:not(.t2)", new CssSelector { Classes = { "t1" }, PseudoClasses = { "not(.t2)" } });
				yield return new TestCaseData(":not(.t2)", new CssSelector { PseudoClasses = { "not(.t2)" }});
				yield return new TestCaseData("input:enabled", new CssSelector { PseudoClasses = { "enabled" }});


			}
		}

		public static IEnumerable AttributesDataSource
		{
			get
			{
				yield return new TestCaseData("p[title]", new CssSelector
				{
					Tag = "p",
					Attributes =
					{
						new CssAttribute
						{
							Key = "title",
							MathingRule = CssAttributeMathcingRule.Exists
						}
					}
				});
				yield return new TestCaseData("p[title=\"a\"]", new CssSelector
				{
					Tag = "p",
					Attributes =
					{
						new CssAttribute
						{
							Key = "title",
							Value = "a",
							MathingRule = CssAttributeMathcingRule.Equals
						}
					}
				});
				yield return new TestCaseData("p[title~=\"a\"]", new CssSelector
				{
					Tag = "p",
					Attributes =
					{
						new CssAttribute
						{
							Key = "title",
							Value = "a",
							MathingRule = CssAttributeMathcingRule.ContainsWord
						}
					}
				});
				yield return new TestCaseData("p[title|=\"a\"]", new CssSelector
				{
					Tag = "p",
					Attributes =
					{
						new CssAttribute
						{
							Key = "title",
							Value = "a",
							MathingRule = CssAttributeMathcingRule.ContainsPrefix
						}
					}
				});
				yield return new TestCaseData("p[title^=\"a\"]", new CssSelector
				{
					Tag = "p",
					Attributes =
					{
						new CssAttribute
						{
							Key = "title",
							Value = "a",
							MathingRule = CssAttributeMathcingRule.BeginsWith
						}
					}
				});
				yield return new TestCaseData("p[title$=\"a\"]", new CssSelector
				{
					Tag = "p",
					Attributes =
					{
						new CssAttribute
						{
							Key = "title",
							Value = "a",
							MathingRule = CssAttributeMathcingRule.EndsWith
						}
					}
				});
				yield return new TestCaseData("p[title*=\"a\"]", new CssSelector
				{
					Tag = "p",
					Attributes =
					{
						new CssAttribute
						{
							Key = "title",
							Value = "a",
							MathingRule = CssAttributeMathcingRule.Contains
						}
					}
				});
			}
		}
	}





	public static class AssertEx
	{

		public static void PropertyValuesAreEquals(object actual, object expected)
		{
			PropertyInfo[] properties = expected.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
			{
				object expectedValue = property.GetValue(expected, null);
				object actualValue = property.GetValue(actual, null);

				if (actualValue is IList)
					AssertListsAreEquals(property, (IList)actualValue, (IList)expectedValue);
				else if (!Equals(expectedValue, actualValue))
					Assert.Fail("Property {0}.{1} does not match. Expected: {2} but was: {3}", property.DeclaringType.Name, property.Name, expectedValue, actualValue);
			}
		}

		private static void AssertListsAreEquals(PropertyInfo property, IList actualList, IList expectedList)
		{
			if (actualList.Count != expectedList.Count)
				Assert.Fail("Property {0}.{1} does not match. Expected IList containing {2} elements but was IList containing {3} elements", property.PropertyType.Name, property.Name, expectedList.Count, actualList.Count);

			for (int i = 0; i < actualList.Count; i++)
				if (!Equals(actualList[i], expectedList[i]))
					Assert.Fail("Property {0}.{1} does not match. Expected IList with element {1} equals to {2} but was IList with element {1} equals to {3}", property.PropertyType.Name, property.Name, expectedList[i], actualList[i]);
		}
	}
}
