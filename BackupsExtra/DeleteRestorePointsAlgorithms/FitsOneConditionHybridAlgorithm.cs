using System;
using System.Linq;
using Backups.BackupAbstractModel;

namespace BackupsExtra.DeleteRestorePointsAlgorithms
{
    public class FitsOneConditionHybridAlgorithm : HybridAlgorithm
    {
        public FitsOneConditionHybridAlgorithm(int maxPoints, DateTime date)
            : base(maxPoints, date)
        {
        }

        public override void Clean(BackupJob job)
        {
            var points = job.Points.ToList();
            points.Where(x => PointMustBeDeleted(x, job)).ToList().ForEach(point => job.DeleteRestorePoint(point));
        }

        public override bool PointMustBeDeleted(RestorePoint point, BackupJob job)
        {
            return UpToDateAlgorithm.PointMustBeDeleted(point, job) ||
                   FixedAmountAlgorithm.PointMustBeDeleted(point, job);
        }
    }
}