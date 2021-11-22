using System;
using Backups.BackupAbstractModel;

namespace BackupsExtra.DeleteRestorePointsAlgorithms
{
    public abstract class HybridAlgorithm : ICleanerAlgorithm
    {
        public HybridAlgorithm(int maxPoints, DateTime date)
        {
            UpToDateAlgorithm = new UpToDateAlgorithm(date);
            FixedAmountAlgorithm = new FixedAmountAlgorithm(maxPoints);
        }

        protected UpToDateAlgorithm UpToDateAlgorithm { get; }
        protected FixedAmountAlgorithm FixedAmountAlgorithm { get; }
        public abstract void Clean(BackupJob job);
        public abstract bool PointMustBeDeleted(RestorePoint point, BackupJob job);
    }
}