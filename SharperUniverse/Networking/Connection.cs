using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using SharperUniverse.Networking.EventArguments;

namespace SharperUniverse.Networking
{
    internal class Connection : ISharperConnection
    {
        public Guid Id { get; }
        public byte[] Data { get; } = new byte[1024];

        private readonly Socket _socket;

        public event EventHandler<MessageReceivedArgs> ReceivedMessage;

        public Connection(Socket socket)
        {
            _socket = socket;
            Id = new Guid();
        }

        public void Send(byte[] data)
        {
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, OnSendComplete, null);
        }

        public void Send(string data)
        {
            Send(Encoding.ASCII.GetBytes(data));
        }

        public void ListenForData()
        {
            _socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, OnDataReceived, null);
        }

        private void OnSendComplete(IAsyncResult asyncResult)
        {
            _socket.EndSend(asyncResult);
        }

        private void OnDataReceived(IAsyncResult asyncResult)
        {
            if (!_socket.Connected) return;

            var messageLength = _socket.EndReceive(asyncResult);

            if (messageLength == 0) Disconnect();

            var data = ParseData();

            // TODO: Create new eventarg for ReceivedMessage that contains the message received.
            if (!string.IsNullOrEmpty(data)) ReceivedMessage?.Invoke(this, new MessageReceivedArgs(data));

            ListenForData();
        }

        private string ParseData()
        {
            // TODO: implement protocol parsing here
            return "";
        }

        public void Disconnect()
        {
            if (!_socket.Connected) return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}
