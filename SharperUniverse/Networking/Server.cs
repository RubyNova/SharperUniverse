using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SharperUniverse.Networking.EventArguments;

namespace SharperUniverse.Networking
{
    internal class Server : ISharperServer
    {
        public int Port { get; }

        public ConcurrentBag<ISharperConnection> Connections { get; private set; }

        public event EventHandler NewConnectionMade;
        
        private Socket _mainSocket;

        public Server(int port = 23)
        {
            Port = port;
        }

        public bool Start()
        {
            Connections = new ConcurrentBag<ISharperConnection>();

            _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localIp = new IPEndPoint(IPAddress.Any, Port);

            _mainSocket.Bind(localIp);

            _mainSocket.Listen(4);

            _mainSocket.BeginAccept(OnClientConnect, null);

            return true;
        }

        public void Stop()
        {
            _mainSocket?.Close();

            Parallel.ForEach(Connections, connection =>
            {
                connection.Send("Server is closing down!");
                connection.Disconnect();
            });
        }

        private void OnClientConnect(IAsyncResult asyncResult)
        {
            // Stop accepting new connections and creat a specific connection for the client.
            var socket = _mainSocket.EndAccept(asyncResult);
            ISharperConnection connection = new Connection(socket);
            Connections.Add(connection);

            connection.ListenForData();

            NewConnectionMade?.Invoke(this, new NewConnectionArgs(connection));

            // Start accepting new connections again.
            _mainSocket.BeginAccept(OnClientConnect, null);
        }
    }
}
