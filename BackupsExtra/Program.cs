using System.Threading;
using Backups.BackupAbstractModel;
using Backups.LocalBackup;
using BackupsExtra.CleanUpPoints;
using BackupsExtra.DeleteRestorePointsAlgorithms;
using BackupsExtra.MergePoints;
using BackupsExtra.SaveChanges;

namespace BackupsExtra
{
    internal class Program
    {
        private static void Main()
        {
            // TestDeleteRestorePointsLocally();
            TestLocalMerge();
        }

        private static void TestDeleteRestorePointsLocally()
        {
            string path = "/Users/vekajp/Desktop/test_backup";
            var localRepository = new LocalRepository(path);
            var joba = new BackupJob("jobNotCleaned", new SplitStorage(), localRepository);
            var file1 = new LocalFile("/Users/vekajp/Desktop/backups/file1.txt");
            var file2 = new LocalFile("/Users/vekajp/Desktop/backups/file2.txt");
            var file3 = new LocalFile("/Users/vekajp/Desktop/backups/file2.png");
            var file4 = new LocalFile("/Users/vekajp/Desktop/backups/file1.png");
            joba.AddObject(file1);
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            joba.AddObject(file2);
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            joba.AddObject(file3);
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            joba.AddObject(file4);
            joba.CreateRestorePoint();

            var jobaCleaned = new BackupJob("jobCleaned", new SingleStorage(), localRepository);
            jobaCleaned.AddObject(file1);
            jobaCleaned.CreateRestorePoint();
            Thread.Sleep(1000);
            jobaCleaned.AddObject(file2);
            jobaCleaned.CreateRestorePoint();
            Thread.Sleep(1000);
            jobaCleaned.AddObject(file3);
            jobaCleaned.CreateRestorePoint();
            Thread.Sleep(1000);
            jobaCleaned.AddObject(file4);
            jobaCleaned.CreateRestorePoint();

            var algorithm = new FixedAmountAlgorithm(3);
            RestorePointsCleaner cleaner = new RestorePointsCleaner(algorithm)
                .AddSaver(new LocalChangeSaver());
            cleaner.Clean(jobaCleaned);
        }

        private static void TestLocalMerge()
        {
            string path = "/Users/vekajp/Desktop/test_backup";
            var localRepository = new LocalRepository(path);
            var joba = new BackupJob("jobNotCleaned", new SplitStorage(), localRepository);
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

            RestorePointsMerger merger = new RestorePointsMerger()
                .AddSaver(new LocalChangeSaver());

            merger.Merge(joba, point1, point2);
            merger.Merge(joba, point3, point4);
        }
    }
}
