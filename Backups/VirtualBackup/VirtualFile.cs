using System;
using System.Collections.Generic;
using System.IO;
using Backups.BackupAbstractModel;

namespace Backups.VirtualBackup
{
    public class VirtualFile : IJobObject
    {
        private string _path;
        public VirtualFile(string path)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public string GetPath()
        {
            return _path;
        }

        public string GetName()
        {
            return Path.GetFileName(_path);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            var other = (VirtualFile)obj;
            return other._path == _path;
        }

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }
    }
}