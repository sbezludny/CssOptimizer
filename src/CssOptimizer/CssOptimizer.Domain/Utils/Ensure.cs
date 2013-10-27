using System;

namespace CssOptimizer.Domain.Utils
{
	public static class Ensure
	{
		public static void NotNullOrEmpty(string argument, string argumentName)
		{
			if (string.IsNullOrEmpty(argument))
				throw new ArgumentNullException(argument, argumentName);
		}
	}
}
