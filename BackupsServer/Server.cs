using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Backups.Server
{
    public class Server
    {
        private const int bytesCount = 1024;
        private TcpListener _server;
        private List<string> _objectsJson;
        public Server(IPAddress address, int port)
        {
            _server = new TcpListener(address, port);
            _objectsJson = new List<string>();
            _server.Start();
            Listen();
            _server.Stop();
        }

        public IReadOnlyCollection<string> GetFiles()
        {
            return _objectsJson;
        }

        private void Listen()
        {
            while (true)
            {
                TcpClient client = _server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                ReceiveFile(stream);
            }
        }
        private void ReceiveFile(Stream stream)
        {
            byte[] size = new byte[sizeof(long)];
            ulong fileSize = BitConverter.ToUInt64(size, 0);
            string json = ReadBytes(stream, fileSize);
            _objectsJson.Add(json);
        }

        private string ReadBytes(Stream stream, ulong quantity)
        {
            var builder = new StringBuilder();
            byte[] text = new byte[bytesCount];
            ulong rest = quantity;
            while (rest > bytesCount)
            {
                stream.Read(text, 0, bytesCount);
                builder.Append(Encoding.ASCII.GetString(text));
                rest -= bytesCount;
            }
            text = new byte[rest];
            stream.Read(text, 0, (int)rest);
            builder.Append(Encoding.ASCII.GetString(text));
            return builder.ToString();
        }
    }
}