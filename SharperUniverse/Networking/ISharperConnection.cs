using System;
using System.Collections.Generic;
using System.Text;
using SharperUniverse.Networking.EventArguments;

namespace SharperUniverse.Networking
{
    public interface ISharperConnection
    {
        Guid Id { get; }

        event EventHandler<MessageReceivedArgs> ReceivedMessage;

        void Send(string data);

        void ListenForData();

        void Disconnect();
    }
}
