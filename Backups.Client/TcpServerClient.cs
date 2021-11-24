using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Backups.Client
{
    public class TcpServerClient : IDisposable
    {
        private const int BytesCount = 1024;
        private TcpClient _client;
        private IPAddress _address;
        private int _port;
        private bool _disposedClient;

        public TcpServerClient(IPAddress address, int port)
        {
            _address = address;
            _port = port;
            _disposedClient = true;
        }

        public void SendData(string stringData)
        {
            Stream stream = ConnectClient();
            byte[] size = BitConverter.GetBytes((long)stringData.Length);
            byte[] data = Encoding.ASCII.GetBytes(stringData);
            stream.Write(size);
            stream.Write(data);
            CloseClient();
        }

        public string[] GetRestorePoints()
        {
            Stream stream = ConnectClient();
            int num = GetInteger(stream);
            string[] names = new string[num];
            for (int i = 0; i < num; i++)
            {
                names[i] = GetString(stream);
            }

            CloseClient();
            return names;
        }

        public string[] GetFilesInRestorePoint(string pointName)
        {
            SendData(pointName);
            Stream stream = ConnectClient();
            int num = GetInteger(stream);
            string[] names = new string[num];

            for (int i = 0; i < num; i++)
            {
                names[i] = GetString(stream);
            }

            CloseClient();
            return names;
        }

        public string GetFile(string fileName, string restorePointName)
        {
            SendData(restorePointName);
            SendData(fileName);

            return GetString();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        private string ReadBytes(Stream stream, int quantity)
        {
            var builder = new StringBuilder();
            byte[] text = new byte[BytesCount];
            int rest = quantity;
            while (rest > BytesCount)
            {
                stream.Read(text, 0, BytesCount);
                builder.Append(Encoding.ASCII.GetString(text));
                rest -= BytesCount;
            }

            text = new byte[rest];
            stream.Read(text, 0, rest);
            builder.Append(Encoding.ASCII.GetString(text));
            return builder.ToString();
        }

        private string GetString()
        {
            Stream stream = ConnectClient();
            return GetString(stream);
        }

        private string GetString(Stream stream)
        {
            int length = GetInteger(stream);
            string result = ReadBytes(stream, length);
            return result;
        }

        private int GetInteger(Stream stream)
        {
            byte[] number = new byte[sizeof(long)];
            stream.Read(number, 0, sizeof(long));
            return (int)BitConverter.ToInt64(number);
        }

        private Stream ConnectClient()
        {
            _client = new TcpClient();
            _client.Connect(_address, _port);
            _disposedClient = false;
            return _client.GetStream();
        }

        private void CloseClient()
        {
            _client.Dispose();
        }
    }
}