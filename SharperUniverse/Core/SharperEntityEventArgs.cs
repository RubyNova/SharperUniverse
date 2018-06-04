using System;
using System.Collections.Generic;
using System.Text;

namespace SharperUniverse.Core
{
    public class SharperEntityEventArgs : EventArgs
    {
        public SharperEntity Entity { get; }

        public SharperEntityEventArgs(SharperEntity entity)
        {
            Entity = entity;
        }
    }
}
