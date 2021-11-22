using System;
using System.Collections.Generic;
using System.Linq;
using Backups.BackupAbstractModel;
using BackupsExtra.SaveChanges;

namespace BackupsExtra.MergePoints
{
    public class RestorePointsMerger
    {
        private List<IChangeSaver> _savers;
        public RestorePointsMerger()
        {
            _savers = new List<IChangeSaver>();
        }

        public RestorePointsMerger AddSaver(IChangeSaver saver)
        {
            if (_savers.Find(x => x.GetType() == saver.GetType()) is null)
            {
                _savers.Add(saver);
            }

            return this;
        }

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
                SaveChanges(job);
                return;
            }

            point1.Objects.Where(x => !point2.Objects.Contains(x))
                .ToList().ForEach(x => point2.AddObject(x));
            SaveChanges(job);
        }

        private void SaveChanges(BackupJob job)
        {
            _savers.ForEach(x => x.Update(job));
        }
    }
}