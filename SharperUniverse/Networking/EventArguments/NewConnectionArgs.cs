using System;
using System.Collections.Generic;
using System.Text;

namespace SharperUniverse.Networking.EventArguments
{
    public class NewConnectionArgs : EventArgs
    {
        public ISharperConnection Connection { get; }

        public NewConnectionArgs(ISharperConnection connection)
        {
            Connection = connection;
        }
    }
}
