using System;
using System.Collections.Generic;

namespace Backups.BackupAbstractModel
{
    public class SingleStorage : IStorage
    {
        private IReadOnlyCollection<IJobObject> _objects;
        public SingleStorage(RestorePoint point)
        {
            if (point is null)
            {
                throw new ArgumentNullException(nameof(point));
            }

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