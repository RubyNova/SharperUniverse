using System;

namespace SharperUniverse.Persistence
{
	public class InvalidSaveStateException : Exception
	{
		public InvalidSaveStateException(string message) : base(message)
		{
		}
	}
}