using System;

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
