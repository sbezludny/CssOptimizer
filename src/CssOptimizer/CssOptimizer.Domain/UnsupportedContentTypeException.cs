using System;

namespace CssOptimizer.Domain
{
	public class UnsupportedContentTypeException : Exception
	{
		public UnsupportedContentTypeException(string message)
			: base(message)
		{
		}
	}
}