using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace CssOptimizer.App
{
	class Options
	{
		[ValueList(typeof(List<string>))]
		public IList<string> Urls { get; set; }

		[Option('o', "output", HelpText = "Путь к файлу с результатами.")]
		public string OutputFile { get; set; }

		public string GetOutputFileName()
		{
			var startupDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			return Path.GetFullPath(Path.Combine(startupDirectory, this.OutputFile));
		}

		[HelpOption]
		public string GetUsage()
		{
			var usage = new StringBuilder();
			usage.AppendLine("CssOptimizer");
			return usage.ToString();
		}
	}

}
