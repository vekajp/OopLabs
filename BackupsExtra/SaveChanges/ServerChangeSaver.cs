using System;
using System.IO;
using System.Linq;
using Backups.BackupAbstractModel;
using BackupsExtra.ExtraModel;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;

namespace BackupsExtra.SaveChanges
{
    public class ServerChangeSaver : IChangeSaver
    {
        public void Update(BackupJob job)
        {
            ServerExtraRepository repository = job.Repository as ServerExtraRepository ?? throw new InvalidCastException();
            string[] pointsInRepo = repository.GetRestorePoints();
            pointsInRepo.Where(point => job.Points.All(x => x.Name != point)).ToList()
                .ForEach(repository.RemoveRestorePoint);
            job.Points.ForEach(point => UpdatePoint(point, repository));
        }

        private void UpdatePoint(RestorePoint point, ServerExtraRepository repository)
        {
            string[] filesInRepo = repository.GetFilesInRestorePoint(point.Name);
            CleanRestorePoint(point, repository, filesInRepo);
            AddFilesToPoint(point, repository, filesInRepo);
        }

        private void CleanRestorePoint(RestorePoint point, ServerExtraRepository repository, string[] filesInRepo)
        {
            filesInRepo.Where(x => point.Objects.All(obj => obj.GetName() != x) && point.Name != Path.GetFileNameWithoutExtension(x))
                .ForEach(file => repository.RemoveFileFromRestorePoint(file, point.Name));
        }

        private void AddFilesToPoint(RestorePoint point, ServerExtraRepository repository, string[] filesInRepo)
        {
            point.Objects.Where(x => filesInRepo.All(file => file != x.GetName() && point.Name != Path.GetFileNameWithoutExtension(file)))
                .ForEach(obj => repository.AddFileToRestorePoint(obj, point));
        }
    }
}