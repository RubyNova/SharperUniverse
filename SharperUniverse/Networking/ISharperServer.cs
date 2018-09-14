using System;
using System.Collections.Concurrent;
using SharperUniverse.Networking.EventArguments;

namespace SharperUniverse.Networking
{
    public interface ISharperServer
    {
        int Port { get; }

        ConcurrentDictionary<Guid, ISharperConnection> Connections { get; }

        event EventHandler<NewConnectionArgs> NewConnectionMade;

        bool Start();

        void Stop();
    }
}
