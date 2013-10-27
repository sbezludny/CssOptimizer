using System;

namespace CssOptimizer.Domain.Exceptions
{
	public class UnsupportedContentTypeException : Exception
	{
		public UnsupportedContentTypeException(string message)
			: base(message)
		{
		}
	}
}