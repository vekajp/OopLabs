using System;
using System.Linq;
using System.Threading;
using Backups.BackupAbstractModel;
using Backups.VirtualBackup;
using BackupsExtra.DeleteRestorePointsAlgorithms;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    public class CleaningAlgorithmsTests
    {
        [Test]
        public void TestFixedAmountAlgorithms()
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
            joba.AddObject(new VirtualFile("file4.png"));
            joba.CreateRestorePoint();
            RestorePoint point4 = joba.Points.Last();
            var fixedAmountAlgorithm = new FixedAmountAlgorithm(3);
            Assert.That(fixedAmountAlgorithm.PointMustBeDeleted(point1, joba));
            Assert.That(!fixedAmountAlgorithm.PointMustBeDeleted(point2, joba));
            Assert.That(!fixedAmountAlgorithm.PointMustBeDeleted(point3, joba));
            Assert.That(!fixedAmountAlgorithm.PointMustBeDeleted(point4, joba));
            
            fixedAmountAlgorithm.Clean(joba);
            
            joba.AddObject(new VirtualFile("file5.png"));
            joba.CreateRestorePoint();
            RestorePoint point5 = joba.Points.Last();
            
            Assert.That(!joba.Points.Contains(point1));
            Assert.Throws<ArgumentException>(() =>
            {
                fixedAmountAlgorithm.PointMustBeDeleted(point1, joba);
            });
            Assert.That(fixedAmountAlgorithm.PointMustBeDeleted(point2, joba));
            Assert.That(!fixedAmountAlgorithm.PointMustBeDeleted(point3, joba));
            Assert.That(!fixedAmountAlgorithm.PointMustBeDeleted(point4, joba));
            Assert.That(!fixedAmountAlgorithm.PointMustBeDeleted(point5, joba));
        }

        [Test]
        public void UpToDateAlgorithmTest()
        {
            var repo = new VirtualRepository("some path");
            var joba = new BackupJob("job1", new SplitStorage(), repo);
            joba.AddObject(new VirtualFile("file1.png"));
            joba.CreateRestorePoint();
            RestorePoint point1 = joba.Points.Last();

            DateTime time1 = DateTime.Now;
            Thread.Sleep(1000);
            joba.AddObject(new VirtualFile("file2.png"));
            joba.CreateRestorePoint();
            
            RestorePoint point2 = joba.Points.Last();
            joba.AddObject(new VirtualFile("file3.png"));
            joba.CreateRestorePoint();
            RestorePoint point3 = joba.Points.Last();
            
            DateTime time2 = DateTime.Now;
            Thread.Sleep(1000);
            joba.AddObject(new VirtualFile("file4.png"));
            joba.CreateRestorePoint();
            RestorePoint point4 = joba.Points.Last();
            
            var upToDateAlgorithm = new UpToDateAlgorithm(time1);
            
            Assert.That(upToDateAlgorithm.PointMustBeDeleted(point1, joba));
            Assert.That(!upToDateAlgorithm.PointMustBeDeleted(point2, joba));
            Assert.That(!upToDateAlgorithm.PointMustBeDeleted(point3, joba));
            Assert.That(!upToDateAlgorithm.PointMustBeDeleted(point4, joba));
            
            upToDateAlgorithm.Clean(joba);

            Assert.That(!joba.Points.Contains(point1));
            Assert.Throws<ArgumentException>(() =>
            {
                upToDateAlgorithm.PointMustBeDeleted(point1, joba);
            });

            upToDateAlgorithm = new UpToDateAlgorithm(time2);
            upToDateAlgorithm.Clean(joba);
            Assert.That(!joba.Points.Contains(point2));
            Assert.That(!joba.Points.Contains(point3));
            Assert.That(joba.Points.Contains(point4));
        }

        [Test]
        public void TestHybridFitsOneCondition()
        {
            var repo = new VirtualRepository("some path");
            var joba = new BackupJob("job1", new SplitStorage(), repo);
            joba.AddObject(new VirtualFile("file1.png"));
            joba.CreateRestorePoint();
            RestorePoint point1 = joba.Points.Last();

            
            joba.AddObject(new VirtualFile("file2.png"));
            joba.CreateRestorePoint();
            RestorePoint point2 = joba.Points.Last();
            
            joba.AddObject(new VirtualFile("file3.png"));
            joba.CreateRestorePoint();
            RestorePoint point3 = joba.Points.Last();
            
            DateTime time = DateTime.Now;
            Thread.Sleep(1000);
            joba.AddObject(new VirtualFile("file4.png"));
            joba.CreateRestorePoint();
            RestorePoint point4 = joba.Points.Last();

            var algorithm = new FitsOneConditionHybridAlgorithm(3, time);
            
            Assert.That(algorithm.PointMustBeDeleted(point1, joba));
            Assert.That(algorithm.PointMustBeDeleted(point2, joba));
            Assert.That(algorithm.PointMustBeDeleted(point3, joba));
            Assert.That(!algorithm.PointMustBeDeleted(point4, joba));
            
            algorithm.Clean(joba);
            
            Assert.That(!joba.Points.Contains(point1));
            Assert.That(!joba.Points.Contains(point2));
            Assert.That(!joba.Points.Contains(point3));
            Assert.That(joba.Points.Contains(point4));
            
            joba.AddObject(new VirtualFile("file5.png"));
            joba.CreateRestorePoint();
            RestorePoint point5 = joba.Points.Last();
            
            Assert.That(!algorithm.PointMustBeDeleted(point4, joba));
            Assert.That(!algorithm.PointMustBeDeleted(point5, joba));
            
            algorithm.Clean(joba);
            
            Assert.That(joba.Points.Contains(point4));
            Assert.That(joba.Points.Contains(point5));
        }
        
        [Test]
        public void TestHybridFitsAllConditions()
        {
            var repo = new VirtualRepository("some path");
            var joba = new BackupJob("job1", new SplitStorage(), repo);
            joba.AddObject(new VirtualFile("file1.png"));
            joba.CreateRestorePoint();
            RestorePoint point1 = joba.Points.Last();

            
            joba.AddObject(new VirtualFile("file2.png"));
            joba.CreateRestorePoint();
            RestorePoint point2 = joba.Points.Last();
            
            joba.AddObject(new VirtualFile("file3.png"));
            joba.CreateRestorePoint();
            RestorePoint point3 = joba.Points.Last();
            
            DateTime time = DateTime.Now;
            Thread.Sleep(1000);
            joba.AddObject(new VirtualFile("file4.png"));
            joba.CreateRestorePoint();
            RestorePoint point4 = joba.Points.Last();

            var algorithm = new FitsAllConditionsHybridAlgorithm(3, time);
            
            Assert.That(algorithm.PointMustBeDeleted(point1, joba));
            Assert.That(!algorithm.PointMustBeDeleted(point2, joba));
            Assert.That(!algorithm.PointMustBeDeleted(point3, joba));
            Assert.That(!algorithm.PointMustBeDeleted(point4, joba));
            
            algorithm.Clean(joba);
            
            Assert.That(!joba.Points.Contains(point1));
            Assert.That(joba.Points.Contains(point2));
            Assert.That(joba.Points.Contains(point3));
            Assert.That(joba.Points.Contains(point4));
            
            joba.AddObject(new VirtualFile("file5.png"));
            joba.CreateRestorePoint();
            RestorePoint point5 = joba.Points.Last();
            
            Assert.That(algorithm.PointMustBeDeleted(point2, joba));
            Assert.That(!algorithm.PointMustBeDeleted(point3, joba));
            Assert.That(!algorithm.PointMustBeDeleted(point4, joba));
            Assert.That(!algorithm.PointMustBeDeleted(point5, joba));
            
            algorithm.Clean(joba);
            
            Assert.That(!joba.Points.Contains(point2));
            Assert.That(joba.Points.Contains(point3));
            Assert.That(joba.Points.Contains(point4));
            Assert.That(joba.Points.Contains(point5));
        }
    }
}