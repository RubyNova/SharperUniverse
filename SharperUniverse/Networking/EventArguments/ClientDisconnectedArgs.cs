using System;

namespace SharperUniverse.Networking.EventArguments
{
    public class ClientDisconnectedArgs : EventArgs
    {
        public Guid Id { get; }

        public ClientDisconnectedArgs(Guid id)
        {
            Id = id;
        }
    }
}
