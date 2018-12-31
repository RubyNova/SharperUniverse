using System;

namespace SharperUniverse.Persistence
{
	/// <summary>
	/// Thrown when an invalid state is passed to the provider.
	/// </summary>
	public class InvalidSaveStateException : Exception
	{
		public InvalidSaveStateException(string message = "The save state passed was invalid.") : base(message)
		{
		}
	}
}