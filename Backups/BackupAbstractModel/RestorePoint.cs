using System;
using System.Collections.Generic;
using System.Linq;

namespace Backups.BackupAbstractModel
{
    public class RestorePoint
    {
        private const string DateFormat = "yyyy-MM-dd(HH:mm:ss)";
        private List<IJobObject> _objects;
        public RestorePoint()
        {
        }

        public RestorePoint(List<IJobObject> objects, string jobName, DateTime timeCreated = default)
        {
            DateCreated = timeCreated == default ? DateTime.Now : timeCreated;
            _objects = objects ?? throw new ArgumentNullException(nameof(objects));
            ParentJobName = jobName ?? throw new ArgumentNullException(nameof(jobName));
        }

        public IReadOnlyCollection<IJobObject> Objects => _objects;
        public string Name => DateCreated.ToString(DateFormat);

        public string ParentJobName { get; private set; }
        public DateTime DateCreated { get; private set; }

        public void AddObject(IJobObject jobObject)
        {
            _objects.Add(jobObject);
        }
    }
}