using System;
using System.Collections.Generic;
using System.Linq;
using Backups.BackupAbstractModel;
using Backups.VirtualBackup;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;

namespace BackupsExtra.SaveChanges
{
    public class VirtualChangeSaver : IChangeSaver
    {
        public void Update(BackupJob job)
        {
            VirtualRepository repository = job.Repository as VirtualRepository ?? throw new InvalidCastException();
            IReadOnlyCollection<string> pointsInRepo = repository.Points;
            pointsInRepo.Where(point => job.Points.All(x => x.Name != point)).ToList().ForEach(repository.RemoveRestorePoint);
            job.Points.ForEach(point => CleanRestorePoint(point, repository));
            job.Points.ForEach(point => AddFilesToPoint(point, repository, job.Storage));
        }

        private void CleanRestorePoint(RestorePoint point, VirtualRepository repo)
        {
            var filesInRepo = repo.Storages[point.Name].ToList();
            filesInRepo.Where(x => point.Objects.All(obj => obj.GetName() != x) && point.Objects.ToString() != x)
                .ForEach(file => repo.Storages[point.Name].Remove(file));
        }

        private void AddFilesToPoint(RestorePoint point, VirtualRepository repo, IStorage storage)
        {
            repo.MakeRepository(point);
            storage.StorePoint(point);
            storage.Store(repo);

            // point.Objects.Where(x => filesInRepo.All(file => file != x.GetName() && point.Objects.GetHashCode().ToString() != file))
            //     .ForEach(repo.Store);
        }
    }
}