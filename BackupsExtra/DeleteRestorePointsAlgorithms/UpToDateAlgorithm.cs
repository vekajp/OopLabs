using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Backups.BackupAbstractModel;

namespace BackupsExtra.DeleteRestorePointsAlgorithms
{
    public class UpToDateAlgorithm : ICleanerAlgorithm
    {
        private const string DateFormat = "yyyy-MM-dd(HH:mm:ss)";
        private readonly DateTime _date;
        public UpToDateAlgorithm(DateTime date)
        {
            _date = date;
        }

        public void Clean(BackupJob job)
        {
            var points = job.Points.ToList();
            points.Where(x => DateTime.ParseExact(x.Name, DateFormat, CultureInfo.InvariantCulture) < _date)
                .ToList().ForEach(job.DeleteRestorePoint);
        }

        public bool PointMustBeDeleted(RestorePoint point, BackupJob job)
        {
            var points = job.Points.ToList();
            if (!points.Contains(point))
            {
                throw new ArgumentException("Point is not in the job", nameof(point));
            }

            return DateTime.ParseExact(point.Name, DateFormat, CultureInfo.InvariantCulture) < _date;
        }
    }
}