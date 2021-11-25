using System;
using System.Collections.Generic;
using Backups.BackupAbstractModel;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;

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
            restorePoint.Objects.ForEach(x => Objects?.Add(new JobObjectDto(x)));
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