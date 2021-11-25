using System;
using System.Net;
using Backups.BackupAbstractModel;
using Backups.Client;
using Backups.LocalBackup;
using Backups.ServerBackup;
using Backups.VirtualBackup;

namespace BackupsExtra.ApplicationState
{
    public class RepositoryDto
    {
        public RepositoryDto()
        {
        }

        public RepositoryDto(IRepository repository)
        {
            Path = repository.GetStoragePath();
            if (repository.GetType() != typeof(ServerRepository))
                return;
            ServerRepository repo = repository as ServerRepository ?? throw new InvalidCastException();
            IpAddress = repo.GetClient().Address.ToString();
            Port = repo.GetClient().Port;
        }

        public BackupLocationType Type { get; set; }

        public string Path { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public IRepository GetRepository()
        {
            return Type switch
            {
                BackupLocationType.Local => new LocalRepository(Path),
                BackupLocationType.Virtual => new VirtualRepository(Path),
                BackupLocationType.Server => new ServerRepository(new TcpServerClient(IPAddress.Parse(IpAddress), Port)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}