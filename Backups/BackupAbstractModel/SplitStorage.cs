using System;
using System.Collections.Generic;

namespace Backups.BackupAbstractModel
{
    public class SplitStorage : IStorage
    {
        private IReadOnlyCollection<IJobObject> _objects;
        public SplitStorage(RestorePoint point)
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

            foreach (IJobObject obj in _objects)
            {
                repo.Store(obj);
            }
        }
    }
}