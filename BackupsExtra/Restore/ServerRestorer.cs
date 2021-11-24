using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Backups.BackupAbstractModel;
using Backups.ServerBackup;
using BackupsExtra.ExtraModel;

namespace BackupsExtra.Restore
{
    public class ServerRestorer : IRestorer
    {
        private bool _storeToOriginalLocation;
        private string _storagePath;
        private ServerExtraRepository _repository;
        public ServerRestorer(IRepository repository)
        {
            _repository = repository as ServerExtraRepository ?? throw new InvalidCastException();
            _storeToOriginalLocation = true;
        }

        public IRestorer StoreToLocation(string restoreDirectoryPath)
        {
            Directory.CreateDirectory(restoreDirectoryPath);
            _storeToOriginalLocation = false;
            _storagePath = restoreDirectoryPath;
            return this;
        }

        public void Restore(RestorePoint point)
        {
            string[] fileNames = _repository.GetFilesInRestorePoint(point.Name);
            foreach (string fileName in fileNames)
            {
                string json = _repository.GetFile(point.Name, fileName);
                ServerFile file = JsonSerializer.Deserialize<ServerFile>(json);
                Restore(file);
            }
        }

        public IRestorer SetRepository(IRepository repository)
        {
            _repository = repository as ServerExtraRepository ?? throw new InvalidCastException();
            return this;
        }

        private void Restore(ServerFile file)
        {
            string path = $"{_storagePath}/{file.Name}";
            if (_storeToOriginalLocation)
            {
                path = file.LocalPath;
            }

            using var streamWriter = new StreamWriter(path);
            streamWriter.Write(Encoding.ASCII.GetString(file.Contents));
        }
    }
}