using System;
using System.Collections.Generic;
using System.Text;

namespace SharperUniverse.Networking.EventArguments
{
    public class MessageReceivedArgs : EventArgs
    {
        public string Message { get; }

        public MessageReceivedArgs(string message)
        {
            Message = message;
        }
    }
}
