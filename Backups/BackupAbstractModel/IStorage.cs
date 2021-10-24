namespace Backups.BackupAbstractModel
{
    public interface IStorage
    {
        void StorePoint(RestorePoint point);
        void Store(IRepository repo);
    }
}