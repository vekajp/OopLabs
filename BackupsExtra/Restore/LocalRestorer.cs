using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Backups.BackupAbstractModel;
using BackupsExtra.Logging;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;

namespace BackupsExtra.Restore
{
    public class LocalRestorer : IRestorer
    {
        private bool _storeToOriginalLocation;
        private string _storagePath;
        private IRepository _repository;
        public LocalRestorer(IRepository repository)
        {
            _repository = repository;
            _storeToOriginalLocation = true;
        }

        public IRestorer SetRepository(IRepository repository)
        {
            _repository = repository;
            return this;
        }

        public IRestorer StoreToLocation(string restoreDirectoryPath)
        {
            _storeToOriginalLocation = false;
            _storagePath = restoreDirectoryPath;
            if (!Directory.Exists(restoreDirectoryPath))
            {
                Directory.CreateDirectory(restoreDirectoryPath);
            }

            return this;
        }

        public void Restore(RestorePoint point)
        {
            string pointStoragePath = $"{_repository.GetStoragePath()}/{point.ParentJobName}/{point.Name}";
            if (!Directory.Exists(pointStoragePath))
            {
                throw new ArgumentException("Invalid restore configuration");
            }

            string[] filesInPoint = Directory.GetFiles(pointStoragePath);
            string directoryToUnzip = CreateDirectory(pointStoragePath, DateTime.Now.ToString(CultureInfo.InvariantCulture));
            filesInPoint.ForEach(file => ZipFile.ExtractToDirectory(file, directoryToUnzip));
            string[] unzipedFiles = Directory.GetFiles(directoryToUnzip);
            unzipedFiles.ForEach(file => RestoreObject(point, file));
        }

        private void RestoreObject(RestorePoint point, string fileToRestore)
        {
            IJobObject jobObject = point.Objects.FirstOrDefault(obj => obj.GetName() == Path.GetFileName(fileToRestore))
                                   ?? throw new ArgumentException("Object is not fount in restore point", nameof(fileToRestore));
            string destinationPath = _storeToOriginalLocation ? jobObject.GetPath() : $"{_storagePath}/{jobObject.GetName()}";
            MoveFile(fileToRestore, destinationPath);
        }

        private void MoveFile(string source, string destination)
        {
            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            File.Move(source, destination);
        }

        private string CreateDirectory(string parentDirectory, string name)
        {
            string directory = $"{parentDirectory}/{name}";
            Directory.CreateDirectory(directory);
            return directory;
        }
    }
}