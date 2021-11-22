using System.Linq;
using System.Threading;
using Backups.BackupAbstractModel;
using Backups.VirtualBackup;
using BackupsExtra.DeleteRestorePointsAlgorithms;
using BackupsExtra.ExtraModel;
using BackupsExtra.SaveChanges;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    public class VirtualBackupsTest
    {
        private VirtualRepository _repo;
        private BackupConfiguration _config;
        [SetUp]
        public void InitializeConfiguration()
        {
            _repo = new VirtualRepository("some path");
            _config = new BackupConfiguration(_repo)
                .AddSaver(new VirtualChangeSaver());
        }
        [Test]
        public void VirtualChangeSaverTest()
        {
            var joba = new BackupJobExtra("job1", _config);
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

            joba.SaveChanges();
            
            Assert.AreEqual(joba.PointsCount ,3);
            Assert.AreEqual(_repo.StoragesCount(), 9);
            Assert.AreEqual(_repo.Points.Count ,3);
            
            joba.DeleteRestorePoint(point4);
            joba.SaveChanges();
            Assert.AreEqual(joba.PointsCount ,2);
            Assert.AreEqual(_repo.StoragesCount(), 5);
            Assert.AreEqual(_repo.Points.Count ,2);
        }
        [Test]
        public void TestVirtualCleaner()
        {
            var joba = new BackupJobExtra("job1", _config.SetCleaningAlgorithm(new FixedAmountAlgorithm(3)));
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

            joba.CleanPoints().SaveChanges();

            Assert.AreEqual(joba.Points.Count, 3);
            
            // 9 because split storage is used
            Assert.AreEqual(_repo.StoragesCount(), 9);
            Assert.AreEqual(_repo.Points.Count, 3);
            
            joba.AddObject(new VirtualFile("file5.png"));
            joba.CreateRestorePoint();

            joba.CleanPoints().SaveChanges();
            
            Assert.AreEqual(joba.Points.Count, 3);
            Assert.AreEqual(_repo.StoragesCount(), 12);
            Assert.AreEqual(_repo.Points.Count, 3);
        }
        [Test]
        public void VirtualMergeTest()
        {
            var joba = new BackupJobExtra("job1", _config);
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

            
            joba.MergePoints(point1, point2);
            joba.MergePoints(point3, point4);
            joba.SaveChanges();
            
            Assert.AreEqual(joba.PointsCount, 2);
            Assert.AreEqual(_repo.StoragesCount(), 6);
            Assert.AreEqual(_repo.Points.Count, 2);
            
            Thread.Sleep(1000);
            joba.DeleteObject(file4);
            joba.CreateRestorePoint();
            RestorePoint point5 = joba.Points.Last();
            
            Assert.AreEqual(joba.PointsCount, 3);
            Assert.AreEqual(_repo.StoragesCount(), 9);
            Assert.AreEqual(_repo.Points.Count, 3);
            
            joba.MergePoints(point4, point5);
            joba.SaveChanges();
            Assert.AreEqual(joba.PointsCount, 2);
            Assert.AreEqual(_repo.StoragesCount(), 6);
            Assert.AreEqual(_repo.Points.Count, 2);
        }

        [Test]
        public void SingleStorageMerge()
        {
            var joba = new BackupJobExtra("job1", _config.SetStorage(new SingleStorage()));
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

            joba.MergePoints(point1, point2);
            joba.MergePoints(point3, point4);
            joba.SaveChanges();
            
            Assert.AreEqual(joba.PointsCount, 2);
            Assert.AreEqual(_repo.StoragesCount(), 2);
            Assert.AreEqual(_repo.Points.Count, 2);
        }
    }
}