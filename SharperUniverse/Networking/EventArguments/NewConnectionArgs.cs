using System;

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
