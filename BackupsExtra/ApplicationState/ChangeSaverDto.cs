using System;
using BackupsExtra.SaveChanges;

namespace BackupsExtra.ApplicationState
{
    public class ChangeSaverDto
    {
        public ChangeSaverDto()
        {
        }

        public ChangeSaverDto(IChangeSaver saver)
        {
            Type = GetType(saver);
        }

        public BackupLocationType Type { get; set; }

        public IChangeSaver GetSaver()
        {
            return Type switch
            {
                BackupLocationType.Local => new LocalChangeSaver(),
                BackupLocationType.Virtual => new VirtualChangeSaver(),
                BackupLocationType.Server => new ServerChangeSaver(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public BackupLocationType GetType(IChangeSaver saver)
        {
            if (saver.GetType() == typeof(LocalChangeSaver))
                return BackupLocationType.Local;
            if (saver.GetType() == typeof(VirtualChangeSaver))
                return BackupLocationType.Virtual;
            if (saver.GetType() == typeof(ServerChangeSaver))
                return BackupLocationType.Server;
            throw new ArgumentException("Invalid type", nameof(saver));
        }
    }
}