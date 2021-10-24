using System.Net;
using Backups.BackupAbstractModel;
using Backups.Client;
using Backups.LocalBackup;
using Backups.ServerBackup;

namespace Backups
{
    internal class Program
    {
        private static void Main()
        {
            var singleStorage = new SingleStorage();
            var splitStorage = new SplitStorage();

            // Test local backup
            string path = "/Users/vekajp/Desktop/test_backup";
            var localRepository = new LocalRepository(path);
            var joba = new BackupJob("job1", singleStorage, localRepository);
            var file1 = new LocalFile("/Users/vekajp/Desktop/backups/file1.txt");
            var file2 = new LocalFile("/Users/vekajp/Desktop/backups/file2.txt");
            joba.AddObject(file1);
            joba.AddObject(file2);
            joba.CreateRestorePoint();
            joba.DeleteObject(file1);
            joba.CreateRestorePoint();

            // Test single server backup
            var serverFile1 = new ServerFile("/Users/vekajp/Desktop/backups/file1.txt");
            var serverFile2 = new ServerFile("/Users/vekajp/Desktop/backups/file2.txt");
            using var client = new TcpServerClient(IPAddress.Parse("127.0.0.1"), 8888);
            using var serverRepository = new ServerRepository(client);
            joba = new BackupJob("server_job1", singleStorage, serverRepository);
            joba.AddObject(serverFile1);
            joba.AddObject(serverFile2);
            joba.CreateRestorePoint();
            joba.DeleteObject(serverFile1);
            joba.CreateRestorePoint();

            // Test split storage server backup
            joba = new BackupJob("server_job2", splitStorage, serverRepository);
            joba.AddObject(serverFile1);
            joba.AddObject(serverFile2);
            joba.CreateRestorePoint();
            joba.DeleteObject(serverFile1);
            joba.CreateRestorePoint();
        }
    }
}
