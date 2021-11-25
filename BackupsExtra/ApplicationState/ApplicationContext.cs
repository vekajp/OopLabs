using System;
using System.Collections.Generic;
using BackupsExtra.ExtraModel;

namespace BackupsExtra.ApplicationState
{
    [Serializable]
    public class ApplicationContext
    {
        private static ApplicationContext _context;
        private ApplicationContext()
        {
            BackupJobs = new List<BackupJobExtra>();
        }

        public List<BackupJobExtra> BackupJobs { get; set; }

        public static ApplicationContext Instance()
        {
            return _context ??= new ApplicationContext();
        }

        public void AddJobToContext(BackupJobExtra job)
        {
            if (BackupJobs.Contains(job))
            {
                throw new ArgumentException("Job already in a context", nameof(job));
            }

            BackupJobs.Add(job);
        }

        public void RemoveJobFromContext(BackupJobExtra job)
        {
            if (!BackupJobs.Contains(job))
            {
                throw new ArgumentException("Job is not in a context", nameof(job));
            }

            BackupJobs.Remove(job);
        }
    }
}