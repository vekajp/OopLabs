using System.Collections.Generic;
using System.Linq;
using BackupsExtra.ExtraModel;

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
            config.Savers.ToList().ForEach(x => Savers?.Add(new ChangeSaverDto(x)));
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