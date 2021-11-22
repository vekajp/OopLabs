using Backups.BackupAbstractModel;

namespace BackupsExtra.Restore
{
    public interface IRestorer
    {
        IRestorer StoreToLocation(string restoreDirectoryPath);
        void Restore(RestorePoint point);
        IRestorer SetRepository(IRepository repository);
    }
}