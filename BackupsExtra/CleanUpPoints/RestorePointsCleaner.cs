using System.Collections.Generic;
using Backups.BackupAbstractModel;
using BackupsExtra.DeleteRestorePointsAlgorithms;
using BackupsExtra.SaveChanges;

namespace BackupsExtra.CleanUpPoints
{
    public class RestorePointsCleaner
    {
        private List<IChangeSaver> _savers;
        private ICleanerAlgorithm _algorithm;
        public RestorePointsCleaner(ICleanerAlgorithm algorithm)
        {
            _savers = new List<IChangeSaver>();
            _algorithm = algorithm;
        }

        public RestorePointsCleaner AddSaver(IChangeSaver saver)
        {
            if (_savers.Find(x => x.GetType() == saver.GetType()) is null)
            {
                _savers.Add(saver);
            }

            return this;
        }

        public RestorePointsCleaner SetAlgorithm(ICleanerAlgorithm algorithm)
        {
            _algorithm = algorithm;
            return this;
        }

        public void Clean(BackupJob job)
        {
            _algorithm.Clean(job);
            _savers.ForEach(x => x.Update(job));
        }
    }
}