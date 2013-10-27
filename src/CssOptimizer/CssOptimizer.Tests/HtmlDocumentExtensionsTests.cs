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
		private HtmlDocument GetHtmlDocument(string headHtmlSnippet)
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
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(html);
			return htmlDocument;
		}

		[TestCaseSource("CssLinksDataSource")]
		public void ExtractExternalCssSources(string headHtmlSnippet, IEnumerable<string> sources)
		{
			//Arrange
			var htmlDocument = GetHtmlDocument(headHtmlSnippet);

			//Act
			var uriSet = htmlDocument.GetExternalCssUrls();

			//Assert
			Assert.AreEqual(sources.ToJson(), uriSet.ToJson());
		}

		[TestCaseSource("InlineCssSource")]
		public void ExtractInlineCss(string htmlSnippet, string expectedInlineCss)
		{
			//Arrange
			var htmlDocument = GetHtmlDocument(htmlSnippet);

			//Act
			var inlineCss = htmlDocument.GetInlineStyles();

			//Assert
			Assert.AreEqual(expectedInlineCss, inlineCss);
		}

		[TestCaseSource("LinksDataSource")]
		public void ExtractNotExternalLinks(Uri pageUrl, string htmlSnippet, IEnumerable<string> expectedUrls)
		{
			//Arrange
			var htmlDocument = GetHtmlDocument(htmlSnippet);

			//Act
			var links = htmlDocument.GetInternalLinks(pageUrl);

			//Assert
			CollectionAssert.AreEquivalent(expectedUrls, links.Select(z => z.ToString()));
		}

		static IEnumerable CssLinksDataSource
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

		static IEnumerable InlineCssSource
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

		static IEnumerable LinksDataSource()
		{
			var htmlSnippet = @"
					<header class=""header"">
	<div class=""in"">
		<strong class=""logo""><a href=""/"">UAWEB challenge</a></strong>
		<nav class=""globalnav"">
			<ul>
                                <li><a href=""/about"" title=""Опис, правила, жюрі"">Про чемпіонат</a></li>
                                <li><a href=""/calendar"" title=""Календар подій"">Календар</a></li>
                			</ul>
		</nav>
	    		<div class=""member-login js-member-login""><a href=""#"">Вхід для учасників</a></div>
	</div>
</header>
<!-- END header-->

<!-- BEGIN social-float -->
<div class=""social-float"">
	<a href=""https://www.facebook.com/uawebchallenge"" class=""fb"">facebook</a>
	<a href=""http://vk.com/uawebchallenge"" class=""vk"">vkontakte</a>
	<a href=""http://twitter.com/#!/search?q=%23uwcua"" class=""tw"">twitter</a>
</div>
				";



			yield return new TestCaseData(new Uri("http://uawebchallenge.com/"), htmlSnippet, new[]
				{
					"http://uawebchallenge.com/", 
					"http://uawebchallenge.com/about", 
					"http://uawebchallenge.com/calendar", 
				});

			yield return new TestCaseData(new Uri("http://uawebchallenge.com"), htmlSnippet, new[]
				{
					"http://uawebchallenge.com/", 
					"http://uawebchallenge.com/about", 
					"http://uawebchallenge.com/calendar", 
				});
			yield return new TestCaseData(new Uri("http://uawebchallenge.com/about"), htmlSnippet, new[]
				{
					"http://uawebchallenge.com/", 
					"http://uawebchallenge.com/about", 
					"http://uawebchallenge.com/calendar", 
				});
		}
	}
}
