using System;
using System.Collections.Generic;
using System.Linq;
using Backups.BackupAbstractModel;

namespace Backups.VirtualBackup
{
    public class VirtualRepository : IRepository
    {
        private string _path;
        private List<string> _restorePoints;
        public VirtualRepository(string path)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
            Storages = new Dictionary<string, List<string>>();
            _restorePoints = new List<string>();
        }

        public IReadOnlyCollection<string> Points => _restorePoints;
        public Dictionary<string, List<string>> Storages { get; }

        public void MakeRepository(RestorePoint point)
        {
            _ = point ?? throw new ArgumentNullException(nameof(point));
            _path = point.Name;
            if (!_restorePoints.Contains(point.Name))
            {
                _restorePoints.Add(point.Name);
            }

            Storages[_path] = new List<string>();
        }

        public void Store(IJobObject obj)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));
            Storages[_path].Add(obj.GetName());
        }

        public void Store(IReadOnlyCollection<IJobObject> objects)
        {
            _ = objects ?? throw new ArgumentNullException(nameof(objects));
            Storages[_path].Add(objects.GetHashCode().ToString());
        }

        public string GetStoragePath()
        {
            return _path;
        }

        public int StoragesCount()
        {
            return Storages.Values.Sum(storages => storages.Count);
        }

        public void RemoveRestorePoint(string pointName)
        {
            if (!_restorePoints.Contains(pointName))
            {
                throw new ArgumentException("Point is not stored", pointName);
            }

            _restorePoints.Remove(pointName);
            Storages.Remove(pointName);
        }
    }
}