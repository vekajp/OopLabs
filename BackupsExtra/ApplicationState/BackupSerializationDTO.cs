using System.Collections.Generic;
using System.Linq;
using Backups.BackupAbstractModel;
using BackupsExtra.ExtraModel;

namespace BackupsExtra.ApplicationState
{
    public class BackupSerializationDto
    {
        public BackupSerializationDto()
        {
        }

        public BackupSerializationDto(BackupJobExtra job)
        {
            Name = job.Name;
            Configuration = new BackupConfigurationDto(job.Configuration);
            Points = job.Points.Select(x => new RestorePointDto(x)).ToList();
            Objects = job.Objects.Select(x => new JobObjectDto(x)).ToList();
        }

        public string Name { get; set; }
        public BackupConfigurationDto Configuration { get; set; }
        public List<RestorePointDto> Points { get; set; }
        public List<JobObjectDto> Objects { get; set; }

        public BackupJobExtra GetJob()
        {
            var job = new BackupJobExtra(Name, Configuration.GetConfiguration());
            Points.ForEach(x => job.AddRestorePoint(x.GetRestorePoint()));
            Objects.ForEach(x => job.AddObject(x.GetObject()));
            return job;
        }
    }
}