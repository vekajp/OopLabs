using System;
using System.Collections.Generic;
using Backups.BackupAbstractModel;

namespace Backups.VirtualBackup
{
    public class VirtualRepository : IRepository
    {
        private string _path;
        private List<string> _storages;
        public VirtualRepository(string path)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _storages = new List<string>();
        }

        public void MakeRepository(RestorePoint point)
        {
            _ = point ?? throw new ArgumentNullException(nameof(point));
            _path = point.Name;
        }

        public void Store(IJobObject obj)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));
            _storages.Add(obj.GetName());
        }

        public void Store(IReadOnlyCollection<IJobObject> objects)
        {
            _ = objects ?? throw new ArgumentNullException(nameof(objects));
            _storages.Add(objects.ToString());
        }

        public string GetStoragePath()
        {
            return _path;
        }

        public int StoragesCount()
        {
            return _storages.Count;
        }
    }
}