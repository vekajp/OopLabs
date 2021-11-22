using System.Linq;
using System.Threading;
using Backups.BackupAbstractModel;
using Backups.VirtualBackup;
using BackupsExtra.CleanUpPoints;
using BackupsExtra.DeleteRestorePointsAlgorithms;
using BackupsExtra.MergePoints;
using BackupsExtra.SaveChanges;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    public class VirtualBackupsTest
    {
        [Test]
        public void VirtualChangeSaverTest()
        {
            var repo = new VirtualRepository("some path");
            var joba = new BackupJob("job1", new SplitStorage(), repo);
            joba.AddObject(new VirtualFile("file1.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            RestorePoint point1 = joba.Points.Last();
            joba.AddObject(new VirtualFile("file2.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            joba.AddObject(new VirtualFile("file3.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            joba.AddObject(new VirtualFile("file4.png"));
            joba.CreateRestorePoint();
            RestorePoint point4 = joba.Points.Last();
            joba.DeleteRestorePoint(point1);
            var saver = new VirtualChangeSaver();
            saver.Update(joba);
            
            Assert.AreEqual(joba.PointsCount ,3);
            Assert.AreEqual(repo.StoragesCount(), 9);
            Assert.AreEqual(repo.Points.Count ,3);
            
            joba.DeleteRestorePoint(point4);
            saver.Update(joba);
            Assert.AreEqual(joba.PointsCount ,2);
            Assert.AreEqual(repo.StoragesCount(), 5);
            Assert.AreEqual(repo.Points.Count ,2);
        }
        [Test]
        public void TestVirtualCleaner()
        {
            var repo = new VirtualRepository("some path");
            var joba = new BackupJob("job1", new SplitStorage(), repo);
            joba.AddObject(new VirtualFile("file1.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            joba.AddObject(new VirtualFile("file2.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            joba.AddObject(new VirtualFile("file3.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            joba.AddObject(new VirtualFile("file4.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            var algorithm = new FixedAmountAlgorithm(3);
            RestorePointsCleaner cleaner = new RestorePointsCleaner(algorithm)
                .AddSaver(new VirtualChangeSaver());

            cleaner.Clean(joba);
            
            Assert.AreEqual(joba.Points.Count, 3);
            
            // 9 because split storage is used
            Assert.AreEqual(repo.StoragesCount(), 9);
            Assert.AreEqual(repo.Points.Count, 3);
            
            joba.AddObject(new VirtualFile("file5.png"));
            joba.CreateRestorePoint();

            cleaner.Clean(joba);
            
            Assert.AreEqual(joba.Points.Count, 3);
            Assert.AreEqual(repo.StoragesCount(), 12);
            Assert.AreEqual(repo.Points.Count, 3);
        }
        [Test]
        public void VirtualMergeTest()
        {
            var repo = new VirtualRepository("some path");
            var joba = new BackupJob("job1", new SplitStorage(), repo);
            joba.AddObject(new VirtualFile("file1.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            RestorePoint point1 = joba.Points.Last();
            joba.AddObject(new VirtualFile("file2.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            RestorePoint point2 = joba.Points.Last();
            joba.AddObject(new VirtualFile("file3.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            RestorePoint point3 = joba.Points.Last();
            var file4 = new VirtualFile("file4.png");
            joba.AddObject(file4);
            joba.CreateRestorePoint();
            RestorePoint point4 = joba.Points.Last();

            RestorePointsMerger merger = new RestorePointsMerger()
                .AddSaver(new VirtualChangeSaver());
            merger.Merge(joba, point1, point2);
            merger.Merge(joba, point3, point4);
            
            Assert.AreEqual(joba.PointsCount, 2);
            Assert.AreEqual(repo.StoragesCount(), 6);
            Assert.AreEqual(repo.Points.Count, 2);
            
            Thread.Sleep(1000);
            joba.DeleteObject(file4);
            joba.CreateRestorePoint();
            RestorePoint point5 = joba.Points.Last();
            
            Assert.AreEqual(joba.PointsCount, 3);
            Assert.AreEqual(repo.StoragesCount(), 9);
            Assert.AreEqual(repo.Points.Count, 3);
            
            merger.Merge(joba, point4, point5);
            Assert.AreEqual(joba.PointsCount, 2);
            Assert.AreEqual(repo.StoragesCount(), 6);
            Assert.AreEqual(repo.Points.Count, 2);
        }

        [Test]
        public void SingleStorageMerge()
        {
            var repo = new VirtualRepository("some path");
            var joba = new BackupJob("job1", new SingleStorage(), repo);
            joba.AddObject(new VirtualFile("file1.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            RestorePoint point1 = joba.Points.Last();
            joba.AddObject(new VirtualFile("file2.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            RestorePoint point2 = joba.Points.Last();
            joba.AddObject(new VirtualFile("file3.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            RestorePoint point3 = joba.Points.Last();
            joba.AddObject(new VirtualFile("file4.png"));
            joba.CreateRestorePoint();
            RestorePoint point4 = joba.Points.Last();

            RestorePointsMerger merger = new RestorePointsMerger()
                .AddSaver(new VirtualChangeSaver());
            merger.Merge(joba, point1, point2);
            merger.Merge(joba, point3, point4);
            
            Assert.AreEqual(joba.PointsCount, 2);
            Assert.AreEqual(repo.StoragesCount(), 2);
            Assert.AreEqual(repo.Points.Count, 2);
        }
    }
}