using System;
using System.Linq;
using System.Net;
using System.Threading;
using Backups.BackupAbstractModel;
using Backups.Client;
using Backups.LocalBackup;
using Backups.ServerBackup;
using Backups.VirtualBackup;
using BackupsExtra.ApplicationState;
using BackupsExtra.DeleteRestorePointsAlgorithms;
using BackupsExtra.ExtraModel;
using BackupsExtra.Logging;
using BackupsExtra.Restore;
using BackupsExtra.SaveChanges;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;
using BackupJobExtra = BackupsExtra.ExtraModel.BackupJobExtra;

namespace BackupsExtra
{
    internal class Program
    {
        private static void Main()
        {
            TestDeleteRestorePointsLocally();
            TestLocalMerge();
            TestLocalRestoreSingleStorage();
            TestLocalRestoreSplitStorage();
            TestServerChangeSaver();
            TestServerRestore();
            TestSystemSaveAndLoad();
        }

        private static void TestDeleteRestorePointsLocally()
        {
            string path = "/Users/vekajp/Desktop/test_backup";
            var localRepository = new LocalRepository(path);
            BackupConfiguration config = new BackupConfiguration(localRepository)
                .AddSaver(new LocalChangeSaver())
                .SetLogger(new ConsoleLogger())
                .SetStorage(new SingleStorage())
                .SetCleaningAlgorithm(new FixedAmountAlgorithm(3));

            var joba = new BackupJobExtra("jobNotCleaned", config);
            var file1 = new LocalFile("/Users/vekajp/Desktop/backups/file1.txt");
            var file2 = new LocalFile("/Users/vekajp/Desktop/backups/file2.txt");
            var file3 = new LocalFile("/Users/vekajp/Desktop/backups/file2.png");
            var file4 = new LocalFile("/Users/vekajp/Desktop/backups/file1.png");
            joba.AddObject(file1);
            joba.CreateRestorePoint();
            joba.AddObject(file2);
            joba.CreateRestorePoint();
            joba.AddObject(file3);
            joba.CreateRestorePoint();
            joba.AddObject(file4);
            joba.CreateRestorePoint();

            var jobaCleaned = new BackupJobExtra("jobCleaned", config);
            jobaCleaned.AddObject(file1);
            jobaCleaned.CreateRestorePoint();
            jobaCleaned.AddObject(file2);
            jobaCleaned.CreateRestorePoint();
            jobaCleaned.AddObject(file3);
            jobaCleaned.CreateRestorePoint();
            jobaCleaned.AddObject(file4);
            jobaCleaned.CreateRestorePoint();

            jobaCleaned.CleanPoints().SaveChanges();
        }

        private static void TestLocalMerge()
        {
            string path = "/Users/vekajp/Desktop/test_backup";
            var localRepository = new LocalRepository(path);
            BackupConfiguration config = new BackupConfiguration(localRepository)
                .AddSaver(new LocalChangeSaver())
                .SetLogger(new ConsoleLogger())
                .SetStorage(new SingleStorage());
            var joba = new BackupJobExtra("jobMerged", config);
            var file1 = new LocalFile("/Users/vekajp/Desktop/backups/file1.txt");
            var file2 = new LocalFile("/Users/vekajp/Desktop/backups/file2.txt");
            var file3 = new LocalFile("/Users/vekajp/Desktop/backups/file2.png");
            var file4 = new LocalFile("/Users/vekajp/Desktop/backups/file1.png");
            joba.AddObject(file1);
            RestorePoint point1 = joba.CreateRestorePoint();
            joba.AddObject(file2);
            RestorePoint point2 = joba.CreateRestorePoint();
            joba.AddObject(file3);
            RestorePoint point3 = joba.CreateRestorePoint();
            joba.AddObject(file4);
            RestorePoint point4 = joba.CreateRestorePoint();

            joba.MergePoints(point1, point2);
            joba.MergePoints(point3, point4);
            joba.SaveChanges();
        }

        private static void TestLocalRestoreSingleStorage()
        {
            string path = "/Users/vekajp/Desktop/test_backup";
            var localRepository = new LocalRepository(path);
            BackupConfiguration config = new BackupConfiguration(localRepository)
                .AddSaver(new LocalChangeSaver())
                .SetLogger(new ConsoleLogger())
                .SetStorage(new SingleStorage());
            var joba = new BackupJobExtra("jobMerged", config);
            var file1 = new LocalFile("/Users/vekajp/Desktop/backups/file1.txt");
            var file2 = new LocalFile("/Users/vekajp/Desktop/backups/file2.txt");
            joba.AddObject(file1);
            joba.AddObject(file2);
            RestorePoint point2 = joba.CreateRestorePoint();
            IRestorer restorer = new LocalRestorer(localRepository)
                .StoreToLocation("/Users/vekajp/Desktop/restore_single");
            joba.SetRestorer(restorer).BackupFromRestorePoint(point2);
        }

        private static void TestLocalRestoreSplitStorage()
        {
            string path = "/Users/vekajp/Desktop/test_backup";
            var localRepository = new LocalRepository(path);
            BackupConfiguration config = new BackupConfiguration(localRepository)
                .AddSaver(new LocalChangeSaver())
                .SetLogger(new ConsoleLogger())
                .SetStorage(new SplitStorage());
            var joba = new BackupJobExtra("jobMerged", config);
            var file1 = new LocalFile("/Users/vekajp/Desktop/backups/file1.txt");
            var file2 = new LocalFile("/Users/vekajp/Desktop/backups/file2.txt");
            joba.AddObject(file1);
            joba.AddObject(file2);
            RestorePoint point2 = joba.CreateRestorePoint();
            IRestorer restorer = new LocalRestorer(localRepository)
                .StoreToLocation("/Users/vekajp/Desktop/restore_split");
            joba.SetRestorer(restorer).BackupFromRestorePoint(point2);
        }

        private static void TestServerChangeSaver()
        {
            using var client = new TcpServerClient(IPAddress.Parse("127.0.0.1"), 8888);
            using var repository = new ServerExtraRepository(client);
            BackupConfiguration config = new BackupConfiguration(repository)
                .AddSaver(new ServerChangeSaver())
                .SetLogger(new ConsoleLogger())
                .SetStorage(new SplitStorage());
            var joba = new BackupJobExtra("serverJob", config);
            var file1 = new ServerFile("/Users/vekajp/Desktop/backups/file1.txt");
            var file2 = new ServerFile("/Users/vekajp/Desktop/backups/file2.txt");
            joba.AddObject(file1);
            RestorePoint point1 = joba.CreateRestorePoint();
            Thread.Sleep(1000);
            joba.AddObject(file2);
            RestorePoint point2 = joba.CreateRestorePoint();
            string[] points = repository.GetRestorePoints();
            points.ForEach(Console.WriteLine);
            points.Select(repository.GetFilesInRestorePoint).ForEach(x => x.ForEach(Console.WriteLine));

            joba.DeleteRestorePoint(point1).SaveChanges();
            points = repository.GetRestorePoints();
            points.ForEach(Console.WriteLine);
            points.Select(repository.GetFilesInRestorePoint).ForEach(x => x.ForEach(Console.WriteLine));

            Thread.Sleep(1000);
            joba.DeleteObject(file1);
            joba.CreateRestorePoint();
            joba.SaveChanges();
            points = repository.GetRestorePoints();
            points.ForEach(Console.WriteLine);
            points.Select(repository.GetFilesInRestorePoint).ForEach(x => x.ForEach(Console.WriteLine));
        }

        private static void TestServerRestore()
        {
            using var client = new TcpServerClient(IPAddress.Parse("127.0.0.1"), 8888);
            using var repository = new ServerExtraRepository(client);
            BackupConfiguration config = new BackupConfiguration(repository)
                .AddSaver(new ServerChangeSaver())
                .SetLogger(new ConsoleLogger())
                .SetStorage(new SplitStorage());
            var joba = new BackupJobExtra("serverJob", config);
            var file1 = new ServerFile("/Users/vekajp/Desktop/backups/file1.txt");
            var file2 = new ServerFile("/Users/vekajp/Desktop/backups/file2.txt");

            IRestorer serverRestorer = new ServerRestorer(repository)
                .StoreToLocation("/Users/vekajp/Desktop/server_restore");

            joba.SetRestorer(serverRestorer);
            joba.AddObject(file1);
            joba.AddObject(file2);
            RestorePoint point2 = joba.CreateRestorePoint();

            joba.BackupFromRestorePoint(point2);
        }

        private static void TestSystemSaveAndLoad()
        {
            string path = "/Users/vekajp/Desktop/test_backup";
            var localRepository = new LocalRepository(path);
            BackupConfiguration config1 = new BackupConfiguration(localRepository)
                .AddSaver(new LocalChangeSaver())
                .SetLogger(new ConsoleLogger())
                .SetStorage(new SplitStorage());
            var localJoba1 = new BackupJobExtra("job1", config1);
            var file1 = new LocalFile("/Users/vekajp/Desktop/backups/file1.txt");
            var file2 = new LocalFile("/Users/vekajp/Desktop/backups/file2.txt");
            localJoba1.AddObject(file1);
            localJoba1.CreateRestorePoint();
            localJoba1.AddObject(file2);
            localJoba1.CreateRestorePoint();

            var localJoba2 = new BackupJobExtra("job2", config1);
            localJoba2.AddObject(file1);
            localJoba2.CreateRestorePoint();

            var repo = new VirtualRepository("some path");
            BackupConfiguration virtualConfig = new BackupConfiguration(repo)
                .AddSaver(new VirtualChangeSaver());
            var virtualJoba = new BackupJobExtra("job3", virtualConfig);
            virtualJoba.AddObject(new VirtualFile("file1.png"));
            virtualJoba.CreateRestorePoint();
            Thread.Sleep(1000);
            RestorePoint point1 = virtualJoba.Points.Last();
            virtualJoba.AddObject(new VirtualFile("file2.png"));
            virtualJoba.CreateRestorePoint();
            Thread.Sleep(1000);
            virtualJoba.AddObject(new VirtualFile("file3.png"));
            virtualJoba.CreateRestorePoint();
            Thread.Sleep(1000);
            virtualJoba.AddObject(new VirtualFile("file4.png"));
            virtualJoba.CreateRestorePoint();

            var applicationContext = ApplicationContext.Instance();
            applicationContext.AddJobToContext(localJoba1);
            applicationContext.AddJobToContext(localJoba2);
            applicationContext.AddJobToContext(virtualJoba);

            XmlSaver saver = new XmlSaver();
            saver.Save(applicationContext);

            var contextLoaded = saver.Load();
            var jobs = contextLoaded.BackupJobs;
            foreach (BackupJobExtra job in jobs)
            {
                Console.WriteLine($"{job.Name}'s points:");
                foreach (RestorePoint point in job.Points)
                {
                    Console.WriteLine($"\t{point.Name}'s objects:");
                    foreach (IJobObject obj in point.Objects)
                    {
                        Console.WriteLine($"\t\t{obj.GetName()}");
                    }
                }

                Console.WriteLine($"{job.Name}'s objects:");
                foreach (IJobObject obj in job.Objects)
                {
                    Console.WriteLine($"\t{obj.GetName()}");
                }
            }
        }
    }
}
