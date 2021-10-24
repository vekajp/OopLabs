using System.Collections.Generic;

namespace Backups.BackupAbstractModel
{
    public interface IJobObject
    {
        public string GetPath();
        public string GetName();
    }
}