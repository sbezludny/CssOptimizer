using System;

namespace CssOptimizer.Domain
{
	public class UnsupportedSelectorException : Exception
	{
		public UnsupportedSelectorException(string message)
			: base(message)
		{
		}
	}
}