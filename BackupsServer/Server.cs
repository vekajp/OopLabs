using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using Backups.ServerBackup;
using BackupsExtra.Restore;

namespace Backups.Server
{
    public class Server
    {
        private const int BytesCount = 1024;
        private TcpClient _client;
        private Dictionary<string, List<ServerFile>> _points;
        private TcpListener _server;
        public Server(IPAddress address, int port)
        {
            _server = new TcpListener(address, port);
            _points = new Dictionary<string, List<ServerFile>>();
            _server.Start();
            Listen();
            _server.Stop();
        }

        private void Listen()
        {
            while (true)
            {
                ServerAction action = GetAction();
                HandleCommand(action);
            }
        }

        private ServerFile ReceiveFile()
        {
            string json = GetString();
            ServerFile file = JsonSerializer.Deserialize<ServerFile>(json) ?? throw new SerializationException(nameof(file));
            return file;
        }

        private string ReadBytes(Stream stream, long quantity)
        {
            var builder = new StringBuilder();
            byte[] text = new byte[BytesCount];
            long rest = quantity;
            while (rest > BytesCount)
            {
                stream.Read(text, 0, BytesCount);
                builder.Append(Encoding.ASCII.GetString(text));
                rest -= BytesCount;
            }

            text = new byte[rest];
            stream.Read(text, 0, (int)rest);
            builder.Append(Encoding.ASCII.GetString(text));
            return builder.ToString();
        }

        private void ReceiveRestorePoint()
        {
            string point = GetString();
            _points[point] = new List<ServerFile>();
        }

        private void DeleteRestorePoint()
        {
            string point = GetString();
            _points.Remove(point);
        }

        private void AddFileToRestorePoint()
        {
            string point = GetString();
            ServerFile file = ReceiveFile();
            _points[point].Add(file);
        }

        private void DeleteFileFromRestorePoint()
        {
            string point = GetString();
            string file = GetString();
            ServerFile fileToRemove = _points[point].FirstOrDefault(x => x.GetName() == file);
            _points[point].Remove(fileToRemove);
        }

        private void GetRestorePointsNames()
        {
            _client = _server.AcceptTcpClient();
            Stream stream = _client.GetStream();
            Dictionary<string, List<ServerFile>>.KeyCollection names = _points.Keys;
            byte[] count = BitConverter.GetBytes((long)_points.Keys.Count);
            stream.Write(count);
            names.ToList().ForEach(name => SendString(stream, name));
        }

        private void GetFileNamesInRestorePoint()
        {
            string point = GetString();
            _client = _server.AcceptTcpClient();
            Stream stream = _client.GetStream();
            byte[] count = BitConverter.GetBytes((long)_points[point].Count);
            stream.Write(count);
            List<ServerFile> names = _points[point];
            names.ForEach(file => SendString(stream, file.Name));
        }

        private void GetFile()
        {
            string point = GetString();
            string name = GetString();
            _client = _server.AcceptTcpClient();
            Stream stream = _client.GetStream();
            ServerFile file = _points[point].Find(f => f.Name == name);
            string json = JsonSerializer.Serialize(file);

            SendString(stream, json);
        }

        private ServerAction GetAction()
        {
            string action = GetString();
            return Enum.Parse<ServerAction>(action);
        }

        private string GetString()
        {
            _client = _server.AcceptTcpClient();
            Stream stream = _client.GetStream();
            byte[] size = new byte[sizeof(long)];
            stream.Read(size, 0, sizeof(long));
            long stringLength = BitConverter.ToInt64(size);
            string result = ReadBytes(stream, stringLength);
            return result;
        }

        private void SendString(Stream stream, string str)
        {
            stream.Write(BitConverter.GetBytes((long)str.Length));
            stream.Write(Encoding.ASCII.GetBytes(str));
        }

        private void HandleCommand(ServerAction action)
        {
            switch (action)
            {
                case ServerAction.ReceiveRestorePoint:
                    ReceiveRestorePoint();
                    break;
                case ServerAction.DeleteRestorePoint:
                    DeleteRestorePoint();
                    break;
                case ServerAction.DeleteFileFromRestorePoint:
                    DeleteFileFromRestorePoint();
                    break;
                case ServerAction.AddFileToRestorePoint:
                    AddFileToRestorePoint();
                    break;
                case ServerAction.GetRestorePointsNames:
                    GetRestorePointsNames();
                    break;
                case ServerAction.GetFileNamesInRestorePoint:
                    GetFileNamesInRestorePoint();
                    break;
                case ServerAction.GetServerFile:
                    GetFile();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }
}