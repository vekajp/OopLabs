using System;
using System.Collections.Generic;

namespace Backups.BackupAbstractModel
{
    public class BackupJob
    {
        private string _name;
        private IStorage _storage;
        private IRepository _repository;
        private List<IJobObject> _objects;
        private List<RestorePoint> _points;
        public BackupJob(string name, IStorage storage, IRepository repository)
        {
            _name = name;
            _storage = storage;
            _repository = repository;
            _objects = new List<IJobObject>();
            _points = new List<RestorePoint>();
        }

        public int PointsCount => _points.Count;
        public void AddObject(IJobObject obj)
        {
            if (_objects.Contains(obj))
            {
                throw new ArgumentException("Object is already in a job", nameof(obj));
            }

            _objects.Add(obj);
        }

        public void DeleteObject(IJobObject obj)
        {
            if (!_objects.Contains(obj))
            {
                throw new ArgumentException("Object is not in a job", nameof(obj));
            }

            _objects.Remove(obj);
        }

        public void CreateRestorePoint()
        {
            var point = new RestorePoint(_objects, _name);
            _repository.MakeRepository(point);
            _storage.StorePoint(point);
            _storage.Store(_repository);
            _points.Add(point);
        }

        public void Clear()
        {
            _objects.Clear();
        }
    }
}