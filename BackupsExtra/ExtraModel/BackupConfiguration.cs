using System.Collections.Generic;
using Backups.BackupAbstractModel;
using BackupsExtra.DeleteRestorePointsAlgorithms;
using BackupsExtra.Logging;
using BackupsExtra.SaveChanges;

namespace BackupsExtra.ExtraModel
{
    public class BackupConfiguration
    {
        private List<IChangeSaver> _savers;
        public BackupConfiguration(IRepository repository)
        {
            Storage = new SplitStorage();
            Repository = repository;
            Logger = new ConsoleLogger();
            Cleaner = new FixedAmountAlgorithm(10);
            _savers = new List<IChangeSaver>();
        }

        public IStorage Storage { get; private set; }
        public IRepository Repository { get; private set; }
        public ILogger Logger { get; private set; }
        public ICleanerAlgorithm Cleaner { get; private set; }
        public IReadOnlyCollection<IChangeSaver> Savers => _savers;
        public BackupConfiguration SetStorage(IStorage storage)
        {
            Storage = storage ?? new SplitStorage();
            return this;
        }

        public BackupConfiguration SetLogger(ILogger logger)
        {
            Logger = logger ?? new NullLogger();
            return this;
        }

        public BackupConfiguration SetRepository(IRepository repository)
        {
            Repository = repository;
            return this;
        }

        public BackupConfiguration SetCleaningAlgorithm(ICleanerAlgorithm algorithm)
        {
            Cleaner = algorithm;
            return this;
        }

        public BackupConfiguration AddSaver(IChangeSaver saver)
        {
            if (_savers.Find(x => x.GetType() == saver.GetType()) is null)
            {
                _savers.Add(saver);
            }

            return this;
        }
    }
}