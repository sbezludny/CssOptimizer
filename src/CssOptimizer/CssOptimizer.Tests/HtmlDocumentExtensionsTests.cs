using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssOptimizer.Domain;
using HtmlAgilityPack;
using NUnit.Framework;
using ServiceStack.Text;

namespace CssOptimizer.Tests
{
	[TestFixture]
	public class HtmlDocumentExtensionsTests
	{
		[TestCaseSource("LinksDataSource")]
		public void ExtractExternalCssSources(string headHtmlSnippet, IEnumerable<string> sources)
		{
			//Arrange
			var htmlDocument = GetHtmlDocument(headHtmlSnippet);

			//Act
			var uriSet = htmlDocument.GetExternalCssSources();

			//Assert
			Assert.AreEqual(sources.ToJson(), uriSet.ToJson());
		}

		private static HtmlDocument GetHtmlDocument(string headHtmlSnippet)
		{
			var html = @"
					<!doctype html>
					<html lang=""en"">
						<head>
							<meta charset=""UTF-8"">
							<title>Document</title>
							{0}
						</head>
						<body>
	
						</body>
					</html>
				";

			html = String.Format(html, headHtmlSnippet);
			var htmlDocument = new HtmlAgilityPack.HtmlDocument();
			htmlDocument.LoadHtml(html);
			return htmlDocument;
		}

		[TestCaseSource("InlineCssSource")]
		public void ExtractInlineCss(string htmlSnippet, string expectedInlineCss)
		{
			//Arrange
			var htmlDocument = GetHtmlDocument(htmlSnippet);

			//Act
			var inlineCss = htmlDocument.GetInlineCss();

			//Assert
			Assert.AreEqual(expectedInlineCss, inlineCss);
		}

		public static IEnumerable LinksDataSource
		{
			get
			{
				yield return new TestCaseData(@"
					<link href='http://fonts.googleapis.com/css?family=Open+Sans:400,300,600&subset=latin,cyrillic' rel='stylesheet' type='text/css'>
					<link rel=""stylesheet"" media=""screen"" href=""http://uawebchallenge.com/public/design/css/screen.css?20130327"" >
				", new []
				{
					"http://fonts.googleapis.com/css?family=Open+Sans:400,300,600&subset=latin,cyrillic", 
					"http://uawebchallenge.com/public/design/css/screen.css?20130327"
				});
				yield return new TestCaseData(@"
					<link href='/css/style.css' rel='stylesheet' type='text/css'>
					<link rel=""stylesheet"" media=""screen"" href=""css/screen.css"" >
				", new[]
				{
					"/css/style.css", 
					"css/screen.css"
				});

			}
		}


		public static IEnumerable InlineCssSource
		{
			get
			{
				yield return new TestCaseData(@"
					<style type=""text/css"">
						.style1
						{
							font-style: italic;
							text-decoration: underline;
							font-size:140%;
						}
						.style2
						{
							font-size:75%;
							color:#FF3300;
							font-weight:bold;
						}
		
					</style>
				", @".style1
						{
							font-style: italic;
							text-decoration: underline;
							font-size:140%;
						}
						.style2
						{
							font-size:75%;
							color:#FF3300;
							font-weight:bold;
						}");

			}
		}

	}
}
