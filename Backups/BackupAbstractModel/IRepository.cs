using System.Collections.Generic;

namespace Backups.BackupAbstractModel
{
    public interface IRepository
    {
        void MakeRepository(RestorePoint point);
        void Store(IJobObject obj);
        void Store(IReadOnlyCollection<IJobObject> objects);

        string GetStoragePath();
    }
}