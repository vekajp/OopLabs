using System;
using System.Collections.Generic;

namespace Backups.BackupAbstractModel
{
    public class SingleStorage : IStorage
    {
        private IReadOnlyCollection<IJobObject> _objects;
        public SingleStorage()
        {
            _objects = new List<IJobObject>();
        }

        public void StorePoint(RestorePoint point)
        {
            _ = point ?? throw new ArgumentNullException(nameof(point));
            _objects = point.Objects;
        }

        public void Store(IRepository repo)
        {
            if (repo is null)
            {
                throw new ArgumentNullException(nameof(repo));
            }

            repo.Store(_objects);
        }
    }
}