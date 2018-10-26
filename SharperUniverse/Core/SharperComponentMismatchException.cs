using System;

namespace SharperUniverse.Core
{
    public class SharperComponentMismatchException : Exception
    {
        public SharperComponentMismatchException() : base("An attempt was made to register a component to a system that does not manage that component type.")
        {
            
        }
    }
}