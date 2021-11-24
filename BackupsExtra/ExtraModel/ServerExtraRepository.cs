using System;
using System.Collections.Generic;
using System.Text;
using Backups.BackupAbstractModel;
using Backups.Client;
using Backups.ServerBackup;
using BackupsExtra.Restore;

namespace BackupsExtra.ExtraModel
{
    public class ServerExtraRepository : IRepository, IDisposable
    {
        private ServerRepository _repository;
        private TcpServerClient _client;
        private RestorePoint _lastPoint;
        public ServerExtraRepository(TcpServerClient client)
        {
            _lastPoint = null;
            _client = client;
            _repository = new ServerRepository(client);
        }

        public void MakeRepository(RestorePoint point)
        {
            SendAction(ServerAction.ReceiveRestorePoint);
            _repository.MakeRepository(point);
            _client.SendData(point.Name);
            _lastPoint = point;
        }

        public void Store(IJobObject obj)
        {
             _ = _lastPoint ?? throw new NullReferenceException("no points have been stored yet");
             SendAction(ServerAction.AddFileToRestorePoint);
             _client.SendData(_lastPoint.Name);
             _repository.Store(obj);
        }

        public void Store(IReadOnlyCollection<IJobObject> objects)
        {
            SendAction(ServerAction.AddFileToRestorePoint);
            _client.SendData(_lastPoint.Name);
            _repository.Store(objects);
        }

        public void RemoveRestorePoint(string name)
        {
            SendAction(ServerAction.DeleteRestorePoint);
            _client.SendData(name);
        }

        public void AddFileToRestorePoint(IJobObject obj, RestorePoint point)
        {
            _client.SendData(point.Name);
            Store(obj);
        }

        public void RemoveFileFromRestorePoint(string objName, string pointName)
        {
            SendAction(ServerAction.DeleteFileFromRestorePoint);
            _client.SendData(pointName);
            _client.SendData(objName);
        }

        public string[] GetRestorePoints()
        {
            SendAction(ServerAction.GetRestorePointsNames);
            return _client.GetRestorePoints();
        }

        public string[] GetFilesInRestorePoint(string name)
        {
            SendAction(ServerAction.GetFileNamesInRestorePoint);
            return _client.GetFilesInRestorePoint(name);
        }

        public string GetFile(string pointName, string fileName)
        {
            SendAction(ServerAction.GetServerFile);
            return _client.GetFile(fileName, pointName);
        }

        public string GetStoragePath()
        {
            return _repository.GetStoragePath();
        }

        public void Dispose()
        {
            _repository?.Dispose();
            _client?.Dispose();
        }

        private void SendAction(ServerAction action)
        {
            _client.SendData(action.ToString());
        }
    }
}