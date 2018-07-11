using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SharperUniverse.Networking.EventArguments;

namespace SharperUniverse.Networking
{
    internal class Server : ISharperServer
    {
        public int Port { get; }

        public ConcurrentDictionary<Guid, ISharperConnection> Connections { get; private set; }

        public event EventHandler<NewConnectionArgs> NewConnectionMade;

        private Socket _mainSocket;

        private bool _isRunning;

        public Server(int port = 23)
        {
            Port = port;
        }

        public bool Start()
        {
            if (_isRunning) return false;
            _isRunning = true;

            try
            {
                Connections = new ConcurrentDictionary<Guid, ISharperConnection>();
                
                _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var localIp = new IPEndPoint(IPAddress.Any, Port);

                _mainSocket.Bind(localIp);

                _mainSocket.Listen(4);

                _mainSocket.BeginAccept(OnClientConnect, null);

                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        public void Stop()
        {
            if (!_isRunning) return;

            _mainSocket?.Close();

            Parallel.ForEach(Connections.Values, connection =>
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

            Connections.TryAdd(connection.Id, connection);

            connection.ClientDisconnected += ClientDisconnected;

            connection.ListenForData();

            NewConnectionMade?.Invoke(this, new NewConnectionArgs(connection));

            // Start accepting new connections again.
            _mainSocket.BeginAccept(OnClientConnect, null);
        }

        private void ClientDisconnected(object sender, ClientDisconnectedArgs e)
        {
            Connections.TryRemove(e.Id, out _);
        }
    }
}
