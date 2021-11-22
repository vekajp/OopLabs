using Backups.BackupAbstractModel;
using BackupsExtra.DeleteRestorePointsAlgorithms;

namespace BackupsExtra.CleanUpPoints
{
    public class RestorePointsCleaner
    {
        private ICleanerAlgorithm _algorithm;
        public RestorePointsCleaner(ICleanerAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

        public RestorePointsCleaner SetAlgorithm(ICleanerAlgorithm algorithm)
        {
            _algorithm = algorithm;
            return this;
        }

        public void Clean(BackupJob job)
        {
            _algorithm.Clean(job);
        }
    }
}