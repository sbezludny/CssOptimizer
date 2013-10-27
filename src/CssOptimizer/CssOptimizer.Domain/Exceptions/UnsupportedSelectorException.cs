using System;

namespace CssOptimizer.Domain.Exceptions
{
	public class UnsupportedSelectorException : Exception
	{
		public UnsupportedSelectorException(string message)
			: base(message)
		{
		}
	}
}