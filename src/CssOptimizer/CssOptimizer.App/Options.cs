using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace CssOptimizer.App
{
	class Options
	{
		[ValueList(typeof(List<string>))]
		public IList<string> Urls { get; set; }

		[Option('s', Required = false, HelpText = "Режим анализа сайта.")]
		public bool SiteAnalysisMode { get; set; }

		[Option('c', Required = false, HelpText = "Ограничение количества страниц при анализе сайта.")]
		public int MaximumPages { get; set; }

		[Option('q', Required = false, HelpText = ".")]
		public bool Quite { get; set; }

		[Option('o', Required = false, HelpText = "Путь к файлу с результатами.")]
		public string OutputFile { get; set; }

		public string GetOutputFileName()
		{
			var startupDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			return Path.GetFullPath(Path.Combine(startupDirectory, this.OutputFile));
		}

		[HelpOption('h', HelpText = "Информация о настройках и использовании программы.")]
		public string GetUsage()
		{
			var help = new HelpText
			{
				Heading = new HeadingInfo("CssOptimizer"),
				AdditionalNewLineAfterOption = true,
				AddDashesToOption = true
			};

			help.AddPreOptionsLine("");
			help.AddPreOptionsLine("");

			help.AddPreOptionsLine("Использование: ./CssOptimizer.App.exe url1 [url2...] [-s] [-c] [-o output_file]");

			help.AddPreOptionsLine("");
			help.AddPreOptionsLine("");


			help.AddPreOptionsLine(String.Format("  {0}    {1}", "url1 [url2…]", "Адреса анализируемых страниц."));

			
			help.AddOptions(this);
			return help;
			
			
		}
	}

}
