namespace Backups.BackupAbstractModel
{
    public interface IStorage
    {
        void Store(IRepository repo);
    }
}