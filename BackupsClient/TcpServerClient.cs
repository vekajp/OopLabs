using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BackupsClient
{
    public class TcpServerClient : IDisposable
    {
        private TcpClient _client;
        private IPAddress _address;
        private int _port;
        
        public TcpServerClient(IPAddress address, int port)
        {
            _address = address;
            _port = port;
        }
        public void SendData(string json)
        {
            _client = new TcpClient();
            _client.Connect(_address, _port);
            NetworkStream stream = _client.GetStream();
            byte[] size = BitConverter.GetBytes((long)json.Length);
            byte[] data = Encoding.ASCII.GetBytes(json);
            stream.Write(size);
            stream.Write(data);
            stream.Close();
            _client.Close();
        }
        
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}