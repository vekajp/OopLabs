using Backups.BackupAbstractModel;

namespace BackupsExtra.DeleteRestorePointsAlgorithms
{
    public interface ICleanerAlgorithm
    {
        void Clean(BackupJob job);
        bool PointMustBeDeleted(RestorePoint point, BackupJob job);
    }
}