using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Backups.BackupAbstractModel;

namespace Backups.LocalBackup
{
    public class LocalRepository : IRepository
    {
        private string _storagePath;
        private string _repoPath;
        public LocalRepository(string storagePath)
        {
            _storagePath = storagePath ?? throw new ArgumentNullException(nameof(storagePath));
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }
        }

        public void MakeRepository(RestorePoint point)
        {
            _ = point ?? throw new ArgumentNullException(nameof(point));
            string jobPath = CreateDirectory(_storagePath, point.ParentJobName);
            _repoPath = CreateDirectory(jobPath, point.Name);
        }

        public void Store(IJobObject obj)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));
            MakeZipFile(obj, _repoPath);
        }

        public void Store(IReadOnlyCollection<IJobObject> objects)
        {
            _ = objects ?? throw new ArgumentNullException(nameof(objects));
            string tempDirectory = CreateDirectory(_storagePath, "temp");
            foreach (IJobObject jobObject in objects)
            {
                CopyFile(jobObject, tempDirectory);
            }

            string zipPath = $"{_repoPath}/{Path.GetFileName(_repoPath)}.zip";
            ZipFile.CreateFromDirectory(tempDirectory, zipPath);
            Directory.Delete(tempDirectory, true);
        }

        public string GetStoragePath()
        {
            return _storagePath;
        }

        public void Delete()
        {
            foreach (string directory in Directory.GetDirectories(_storagePath))
            {
                Directory.Delete(directory, true);
            }

            foreach (string file in Directory.GetFiles(_storagePath))
            {
                File.Delete(file);
            }
        }

        private static string CreateDirectory(string path, string name)
        {
            string fullPath = $"{path}/{name}";
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return fullPath;
        }

        private static void MakeZipFile(IJobObject obj, string path)
        {
            string tempDirectory = CreateDirectory(path, Path.GetFileNameWithoutExtension(obj.GetPath()));
            CopyFile(obj, tempDirectory);

            string zipPath = $"{path}/{obj.GetName()}.zip";
            ZipFile.CreateFromDirectory(tempDirectory, zipPath);
            Directory.Delete(tempDirectory, true);
        }

        private static void CopyFile(IJobObject obj, string path)
        {
            string dest = $"{path}/{obj.GetName()}";
            File.Copy(obj.GetPath(), dest);
        }
    }
}