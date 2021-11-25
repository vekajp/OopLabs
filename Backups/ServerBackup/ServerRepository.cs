using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Backups.BackupAbstractModel;
using Backups.Client;
using Backups.LocalBackup;

namespace Backups.ServerBackup
{
    public class ServerRepository : IRepository, IDisposable
    {
        private TcpServerClient _client;
        private LocalRepository _repo;
        public ServerRepository(TcpServerClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _repo = new LocalRepository("temp");
        }

        public void MakeRepository(RestorePoint point)
        {
            _ = point ?? throw new ArgumentNullException(nameof(point));
            _repo.MakeRepository(point);
        }

        public void Store(IJobObject obj)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));
            string json = JsonSerializer.Serialize<ServerFile>(obj as ServerFile);
            _client.SendData(json);
        }

        public void Store(IReadOnlyCollection<IJobObject> objects)
        {
            _ = objects ?? throw new ArgumentNullException(nameof(objects));
            _repo.Store(objects);
            string storePath = _repo.GetStoragePath();
            string[] zipFiles = Directory.GetFiles(storePath);
            if (zipFiles.Length != 1)
            {
                throw new FileLoadException("More than one file in a single storage's directory");
            }

            var file = new ServerFile(zipFiles[0]);
            Store(file);
        }

        public string GetStoragePath()
        {
            return _repo.GetStoragePath();
        }

        public TcpServerClient GetClient()
        {
            return _client;
        }

        public void Dispose()
        {
            _repo.Delete();
            _client.Dispose();
        }
    }
}