using System;

namespace SharperUniverse.Core
{
    public class DuplicateSharperObjectException : Exception
    {
        public DuplicateSharperObjectException() : base("An attempt to add a SharperUniverse object to the game was made when it was already registered. Duplicates are not allowed.")
        {
        }
    }
}
