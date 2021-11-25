using System;
using System.Collections.Generic;
using System.Linq;
using Backups.BackupAbstractModel;

namespace BackupsExtra.ApplicationState
{
    public class RestorePointDto
    {
        public RestorePointDto()
        {
            Objects = new List<JobObjectDto>();
        }

        public RestorePointDto(RestorePoint restorePoint)
        {
            Objects = new List<JobObjectDto>();
            restorePoint.Objects.ToList().ForEach(x => Objects?.Add(new JobObjectDto(x)));
            ParentJobName = restorePoint.ParentJobName;
            TimeCreated = restorePoint.DateCreated;
        }

        public List<JobObjectDto> Objects { get; set; }
        public DateTime TimeCreated { get; set; }
        public string ParentJobName { get; set; }

        public RestorePoint GetRestorePoint()
        {
            var objects = new List<IJobObject>();
            Objects.ForEach(x => objects.Add(x.GetObject()));
            return new RestorePoint(objects, ParentJobName, TimeCreated);
        }
    }
}