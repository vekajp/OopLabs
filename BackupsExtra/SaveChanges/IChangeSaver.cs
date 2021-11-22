using Backups.BackupAbstractModel;

namespace BackupsExtra.SaveChanges
{
    public interface IChangeSaver
    {
        void Update(BackupJob job);
    }
}