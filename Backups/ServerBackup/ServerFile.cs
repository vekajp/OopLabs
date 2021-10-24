using System;
using System.IO;
using System.Text.Json.Serialization;
using Backups.BackupAbstractModel;

namespace Backups.ServerBackup
{
    public class ServerFile : IJobObject
    {
        private string _localPath;
        public ServerFile(string localPath)
        {
            _localPath = localPath ?? throw new ArgumentNullException(nameof(localPath));
            Name = Path.GetFileName(localPath);
            if (!File.Exists(localPath))
            {
                throw new ArgumentException("File doesn't exist", nameof(localPath));
            }

            Contents = File.ReadAllBytes(localPath);
        }

        public ServerFile()
        {
        }

        [JsonInclude]
        public string Name { get; private set; }
        [JsonInclude]
        public byte[] Contents { get; private set; }

        public string GetPath()
        {
            return _localPath;
        }

        public string GetName()
        {
            return Name;
        }
    }
}