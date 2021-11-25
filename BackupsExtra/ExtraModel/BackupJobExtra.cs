using System;
using System.Collections.Generic;
using System.Linq;
using Backups.BackupAbstractModel;
using BackupsExtra.CleanUpPoints;
using BackupsExtra.Logging;
using BackupsExtra.MergePoints;
using BackupsExtra.Restore;
using BackupsExtra.SaveChanges;

namespace BackupsExtra.ExtraModel
{
    public class BackupJobExtra
    {
        private BackupJob _job;
        private ILogger _logger;
        private RestorePointsMerger _merger;
        private RestorePointsCleaner _cleaner;
        private IRestorer _restorer;
        private List<IChangeSaver> _savers;
        public BackupJobExtra()
        {
            _savers = new List<IChangeSaver>();
        }

        public BackupJobExtra(string name, IStorage storage, IRepository repository, ILogger logger)
        {
            _job = new BackupJob(name, storage, repository);
            _logger = logger;
            _savers = new List<IChangeSaver>();
            _restorer = new LocalRestorer(repository);
        }

        public BackupJobExtra(string name, BackupConfiguration config)
        {
            _job = new Backups.BackupAbstractModel.BackupJob(name, config.Storage, config.Repository);
            _logger = config.Logger;
            _merger = new RestorePointsMerger();
            _cleaner = new RestorePointsCleaner(config.Cleaner);
            _savers = config.Savers.ToList();
            Configuration = config;
        }

        public string Name => _job.Name;

        public IReadOnlyCollection<IJobObject> Objects => _job.Objects;
        public int PointsCount => _job.PointsCount;
        public IReadOnlyCollection<RestorePoint> Points => _job.Points;
        public IRepository Repository => _job.Repository;
        public IStorage Storage => _job.Storage;
        public BackupConfiguration Configuration { get; private set; }
        public BackupJobExtra ChangeConfiguration(BackupConfiguration config)
        {
            _job = new Backups.BackupAbstractModel.BackupJob(Name, config.Storage, config.Repository);
            _logger = config.Logger;
            _merger = new RestorePointsMerger();
            _cleaner = new RestorePointsCleaner(config.Cleaner);
            _savers = config.Savers.ToList();
            return this;
        }

        public BackupJobExtra AddObject(IJobObject obj)
        {
            _job.AddObject(obj);
            return this;
        }

        public BackupJobExtra DeleteObject(IJobObject obj)
        {
            _job.DeleteObject(obj);
            return this;
        }

        public BackupJobExtra SetRestorer(IRestorer restorer)
        {
            _restorer = restorer;
            return this;
        }

        public RestorePoint CreateRestorePoint()
        {
            _logger.LogMessage("creating restore point...");
            RestorePoint point = _job.CreateRestorePoint();
            _logger.LogMessage($"point {point.Name} was successfully created");
            return point;
        }

        public BackupJobExtra AddRestorePoint(RestorePoint restorePoint)
        {
            _job.AddRestorePoint(restorePoint);
            return this;
        }

        public BackupJobExtra DeleteRestorePoint(RestorePoint point)
        {
            _logger.LogMessage($"deleting restore point {point.Name}");
            _job.DeleteRestorePoint(point);
            _logger.LogMessage($"point {point.Name} was successfully deleted");
            return this;
        }

        public BackupJobExtra Clear()
        {
            _job.Clear();
            _logger.LogMessage($"backup job {Name} was cleared");
            return this;
        }

        public BackupJobExtra CleanPoints()
        {
            _logger.LogMessage("cleaning restore point chain...");
            _cleaner.Clean(_job);
            _logger.LogMessage("point chain was successfully cleaned");
            return this;
        }

        public BackupJobExtra MergePoints(RestorePoint point1, RestorePoint point2)
        {
            _logger.LogMessage($"merging point \"{point1.Name}\" into \"{point2.Name}\"...");
            _merger.Merge(_job, point1, point2);
            _logger.LogMessage($"points \"{point1.Name}\" and \"{point2.Name}\" were successfully merged");
            return this;
        }

        public BackupJobExtra SaveChanges()
        {
            _savers.ForEach(x => x.Update(_job));
            _logger.LogMessage("changes have been saved");
            return this;
        }

        public BackupJobExtra BackupFromRestorePoint(RestorePoint point)
        {
            _ = _restorer ?? throw new NullReferenceException("Restorer was not assigned yet");
            _logger.LogMessage($"start restoring from point {point.Name}");
            _restorer.Restore(point);
            _logger.LogMessage($"successfully restored from point {point.Name}");
            return this;
        }
    }
}