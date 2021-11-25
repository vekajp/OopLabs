using System;
using Backups.BackupAbstractModel;
using Backups.LocalBackup;
using Backups.ServerBackup;
using Backups.VirtualBackup;

namespace BackupsExtra.ApplicationState
{
    public class JobObjectDto
    {
        public JobObjectDto()
        {
        }

        public JobObjectDto(IJobObject jobObject)
        {
            Path = jobObject.GetPath();
            Type = GetType(jobObject);
        }

        public BackupLocationType Type { get; set; }
        public string Path { get; set; }

        public IJobObject GetObject()
        {
            return Type switch
            {
                BackupLocationType.Local => new LocalFile(Path),
                BackupLocationType.Virtual => new VirtualFile(Path),
                BackupLocationType.Server => new ServerFile(Path),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private BackupLocationType GetType(IJobObject obj)
        {
            if (obj.GetType() == typeof(LocalFile))
                return BackupLocationType.Local;
            if (obj.GetType() == typeof(VirtualFile))
                return BackupLocationType.Virtual;
            if (obj.GetType() == typeof(ServerFile))
                return BackupLocationType.Server;
            throw new ArgumentException("Invalid type", nameof(obj));
        }
    }
}