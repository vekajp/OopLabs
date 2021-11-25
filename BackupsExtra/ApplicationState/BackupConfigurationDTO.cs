using System;
using System.Collections.Generic;
using System.Net;
using Backups.BackupAbstractModel;
using Backups.Client;
using Backups.LocalBackup;
using Backups.ServerBackup;
using Backups.VirtualBackup;
using BackupsExtra.DeleteRestorePointsAlgorithms;
using BackupsExtra.ExtraModel;
using BackupsExtra.Logging;
using BackupsExtra.Restore;
using BackupsExtra.SaveChanges;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;

namespace BackupsExtra.ApplicationState
{
    public class BackupConfigurationDto
    {
        public BackupConfigurationDto()
        {
            Savers = new List<ChangeSaverDto>();
        }

        public BackupConfigurationDto(BackupConfiguration config)
        {
            Repository = new RepositoryDto(config.Repository);
            config.Savers.ForEach(x => Savers?.Add(new ChangeSaverDto(x)));
        }

        public RepositoryDto Repository { get; set; }
        public List<ChangeSaverDto> Savers { get; set; }
        public BackupConfiguration GetConfiguration()
        {
            var config = new BackupConfiguration(Repository.GetRepository());
            Savers.ForEach(x => config.AddSaver(x.GetSaver()));
            return config;
        }
    }
}