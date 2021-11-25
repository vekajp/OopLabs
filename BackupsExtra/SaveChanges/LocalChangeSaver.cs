using System;
using System.IO;
using System.Linq;
using Backups.BackupAbstractModel;
using Backups.LocalBackup;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;

namespace BackupsExtra.SaveChanges
{
    public class LocalChangeSaver : IChangeSaver
    {
        public void Update(BackupJob job)
        {
            LocalRepository repository = job.Repository as LocalRepository ?? throw new InvalidCastException();
            string jobPath = $"{repository.GetStoragePath()}/{job.Name}";
            var pointsInRepo = Directory.GetDirectories(jobPath).ToList();
            pointsInRepo.Where(point => job.Points.All(x => x.Name != Path.GetFileName(point))).ToList()
                .ForEach(x => Directory.Delete(x, true));
            job.Points.ForEach(point => CleanRestorePoint(point, jobPath));
            job.Points.ForEach(point => AddFilesToPoint(point, repository, jobPath));
        }

        private void CleanRestorePoint(RestorePoint point, string jobPath)
        {
            string[] filesInRepo = Directory.GetFiles($"{jobPath}/{point.Name}");
            filesInRepo.Where(x => point.Objects.All(obj => obj.GetName() != x) && point.Name != Path.GetFileNameWithoutExtension(x))
                .ForEach(File.Delete);
        }

        private void AddFilesToPoint(RestorePoint point, IRepository repo, string jobPath)
        {
            string[] filesInRepo = Directory.GetFiles($"{jobPath}/{point.Name}");
            repo.MakeRepository(point);
            point.Objects.Where(x => filesInRepo.All(file => file != x.GetName() && point.Name != Path.GetFileNameWithoutExtension(file)))
                .ForEach(repo.Store);
        }
    }
}