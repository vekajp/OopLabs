using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.BackupAbstractModel;

namespace BackupsExtra.DeleteRestorePointsAlgorithms
{
    public class FixedAmountAlgorithm : ICleanerAlgorithm
    {
        private int _maxAmount;
        public FixedAmountAlgorithm(int maxAmount)
        {
            _maxAmount = maxAmount;
        }

        public void Clean(BackupJob job)
        {
            var points = job.Points.ToList();
            if (points.Count - _maxAmount <= 0) return;
            for (int i = 0; i < points.Count - _maxAmount; i++)
            {
               job.DeleteRestorePoint(points[i]);
            }
        }

        public bool PointMustBeDeleted(RestorePoint point, BackupJob job)
        {
            int index = job.Points.ToList().IndexOf(point);
            if (index < 0)
            {
                throw new ArgumentException("Point is not in the job", nameof(point));
            }

            return index < job.Points.Count - _maxAmount;
        }
    }
}