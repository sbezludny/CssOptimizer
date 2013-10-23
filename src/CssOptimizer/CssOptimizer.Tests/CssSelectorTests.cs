using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CssOptimizer.Domain;
using NUnit.Framework;
using ServiceStack.Text;

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
			Assert.AreEqual(cssSelector.ToJson(), expected.ToJson());
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
							Name = "title",
							Operator = CssAttributeOperator.Exists
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
							Name = "title",
							Value = "a",
							Operator = CssAttributeOperator.Equals
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
							Name = "title",
							Value = "a",
							Operator = CssAttributeOperator.ContainsWord
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
							Name = "title",
							Value = "a",
							Operator = CssAttributeOperator.ContainsPrefix
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
							Name = "title",
							Value = "a",
							Operator = CssAttributeOperator.BeginsWith
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
							Name = "title",
							Value = "a",
							Operator = CssAttributeOperator.EndsWith
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
							Name = "title",
							Value = "a",
							Operator = CssAttributeOperator.Contains
						}
					}
				});
			}
		}
	}





	
}
