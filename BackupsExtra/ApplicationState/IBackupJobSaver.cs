using Backups.BackupAbstractModel;

namespace BackupsExtra.ApplicationState
{
    public interface IBackupJobSaver
    {
        void Save();
        BackupJob Load();
    }
}