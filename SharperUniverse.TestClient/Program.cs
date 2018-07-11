using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharperUniverse.TestClient
{
    class Program
    {
        private static TcpClient _client;
        private static NetworkStream _stream;

        static async Task Main(string[] args)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(IPAddress.Parse(GetLocalIPAddress()), 23);
            _stream = _client.GetStream();

            while (true)
            {
                Task.Run(() => GetInputAsync());
                Task.Run(() => GetOutputAsync());
            }
        }

        private static async Task GetOutputAsync()
        {
            var data = new byte[256];
            var bytes = await _stream.ReadAsync(data, 0, data.Length);
            var result = Encoding.ASCII.GetString(data, 0, bytes);
            await Console.Out.WriteLineAsync(result);
        }

        private static async Task GetInputAsync()
        {
            var result = await Console.In.ReadLineAsync();
            await _stream.WriteAsync(Encoding.ASCII.GetBytes(result));
        }

        static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}