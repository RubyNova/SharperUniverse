using System;

namespace SharperUniverse.Persistence
{
	public class InvalidSaveStateException : Exception
	{
		public InvalidSaveStateException(string message = "The save state passed was invalid.") : base(message)
		{
		}
	}
}