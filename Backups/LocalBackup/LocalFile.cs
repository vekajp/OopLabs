using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Backups.BackupAbstractModel;

namespace Backups.LocalBackup
{
    public class LocalFile : IJobObject
    {
        private string _name;
        private string _fullPath;
        public LocalFile(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                throw new ArgumentException("Invalid path", nameof(fullPath));
            }

            _fullPath = fullPath;
            _name = Path.GetFileName(fullPath);
        }

        public string GetPath()
        {
            return _fullPath;
        }

        public string GetName()
        {
            return _name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(LocalFile))
            {
                return false;
            }

            var other = (LocalFile)obj;
            return other._fullPath.Equals(_fullPath);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_fullPath);
        }
    }
}