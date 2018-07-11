﻿using System;
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

            var data = ParseData(messageLength);
            
            if (!string.IsNullOrEmpty(data)) ReceivedMessage?.Invoke(this, new MessageReceivedArgs(data));

            ListenForData();
        }
        
        private string ParseData(int messageLength)
        {
            var message = new StringBuilder();

            for (var index = 0; index < messageLength; index++)
            {
                var firstBit = Data[index];
                switch (firstBit)
                {
                    case (byte)TelnetVerbs.Iac:
                        // interpret as command
                        var secondBit = Data[++index];
                        switch (secondBit)
                        {
                            case (byte)TelnetVerbs.Iac:
                                //literal IAC = 255 escaped, so append char 255 to string
                                message.Append(secondBit);
                                break;
                            case (byte)TelnetVerbs.Do:
                            case (byte)TelnetVerbs.Dont:
                            case (byte)TelnetVerbs.Will:
                            case (byte)TelnetVerbs.Wont:
                                // reply to all commands with "WONT"
                                var thirdBit = Data[++index];
                                var response = secondBit == (byte)TelnetVerbs.Do ? (byte)TelnetVerbs.Wont : (byte)TelnetVerbs.Dont;

                                if (thirdBit == 3) response = secondBit == (byte)TelnetVerbs.Do ? (byte)TelnetVerbs.Will : (byte)TelnetVerbs.Do;

                                Send(new []{ (byte)TelnetVerbs.Iac , response, thirdBit });
                                break;
                        }
                        break;
                    default:
                        message.Append((char)firstBit);
                        break;
                }
            }
            return message.Length > 0 ? message.ToString() : "";
        }

        public void Disconnect()
        {
            if (!_socket.Connected) return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}