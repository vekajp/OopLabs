using System;
using System.Collections.Generic;

namespace Backups.BackupAbstractModel
{
    public class RestorePoint
    {
        private const string DateFormat = "yyyy-MM-dd(HH:mm:ss)";
        private DateTime _dateCreated;
        private List<IJobObject> _objects;
        public RestorePoint(List<IJobObject> objects, string jobName)
        {
            _dateCreated = DateTime.Now;
            _objects = objects ?? throw new ArgumentNullException(nameof(objects));
            ParentJobName = jobName ?? throw new ArgumentNullException(nameof(jobName));
        }

        public IReadOnlyCollection<IJobObject> Objects => _objects;
        public string Name => _dateCreated.ToString(DateFormat);

        public string ParentJobName { get; }
    }
}