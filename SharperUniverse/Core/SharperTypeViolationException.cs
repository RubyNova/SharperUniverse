using System;
using System.Collections.Generic;
using System.Text;

namespace SharperUniverse.Core
{
    class SharperTypeViolationException : Exception
    {
        public SharperTypeViolationException(string message) : base(message)
        {
            
        }
    }
}
