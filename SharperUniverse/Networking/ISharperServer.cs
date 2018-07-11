using System;
using System.Collections.Concurrent;

namespace SharperUniverse.Networking
{
    public interface ISharperServer
    {
        int Port { get; }

        ConcurrentBag<ISharperConnection> Connections { get; }

        event EventHandler NewConnectionMade;

        bool Start();

        void Stop();
    }
}
