using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Backups.BackupAbstractModel
{
    public class BackupJob
    {
        private IStorage _storage;
        private IRepository _repository;
        private List<IJobObject> _objects;
        private List<RestorePoint> _points;
        public BackupJob(string name, IStorage storage, IRepository repository)
        {
            Name = name;
            _storage = storage;
            _repository = repository;
            _objects = new List<IJobObject>();
            _points = new List<RestorePoint>();
        }

        public string Name { get; }
        public IReadOnlyCollection<IJobObject> Objects => _objects;
        public int PointsCount => _points.Count;
        public IReadOnlyCollection<RestorePoint> Points => _points;
        public IRepository Repository => _repository;
        public IStorage Storage => _storage;
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

        public RestorePoint CreateRestorePoint()
        {
            var objects = new List<IJobObject>(_objects);
            var point = new RestorePoint(objects, Name);
            _repository.MakeRepository(point);
            _storage.StorePoint(point);
            _storage.Store(_repository);
            _points.Add(point);
            return point;
        }

        public void AddRestorePoint(RestorePoint point)
        {
            if (_points.Contains(point))
            {
                throw new ArgumentNullException("point is already in a job", nameof(point));
            }

            _points.Add(point);
        }

        public void DeleteRestorePoint(RestorePoint point)
        {
            if (!_points.Contains(point))
            {
                throw new ArgumentException("Point is not in a job", nameof(point));
            }

            _points.Remove(point);
        }

        public void Clear()
        {
            _objects.Clear();
        }
    }
}