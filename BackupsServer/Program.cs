using System.Net;

namespace Backups.Server
{
    internal static class Program
    {
        private static void Main()
        {
            const string address = "127.0.0.1";
            const int port = 8888;
            var ip = IPAddress.Parse(address);
            var server = new Server(ip, port);
        }
    }
}