using System;
using System.Linq;
using Backups.BackupAbstractModel;

namespace BackupsExtra.MergePoints
{
    public class RestorePointsMerger
    {
        public void Merge(BackupJob job, RestorePoint point1, RestorePoint point2)
        {
            if (!job.Points.Contains(point1))
            {
                throw new ArgumentException("Point is not in a job", nameof(point1));
            }

            if (!job.Points.Contains(point2))
            {
                throw new ArgumentException("Point is not in a job", nameof(point2));
            }

            if (string.CompareOrdinal(point2.Name, point1.Name) < 0)
            {
                (point1, point2) = (point2, point1);
            }

            job.DeleteRestorePoint(point1);
            if (job.Storage.GetType() == typeof(SingleStorage))
            {
                return;
            }

            point1.Objects.Where(x => !point2.Objects.Contains(x))
                .ToList().ForEach(x => point2.AddObject(x));
        }
    }
}