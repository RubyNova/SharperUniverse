using System;
using SharperUniverse.Networking.EventArguments;

namespace SharperUniverse.Networking
{
    public interface ISharperConnection
    {
        Guid Id { get; }

        event EventHandler<MessageReceivedArgs> ReceivedMessage;

        event EventHandler<ClientDisconnectedArgs> ClientDisconnected;

        void Send(string data);

        void ListenForData();

        void Disconnect();
    }
}
