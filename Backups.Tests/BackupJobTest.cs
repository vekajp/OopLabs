using System.Threading;
using Backups.BackupAbstractModel;
using NUnit.Framework;
using Backups.VirtualBackup;

namespace Backups.Tests
{
    public class BackupJobTest
    {
        [Test]
        public void SplitVirtualStorageTest()
        {
            var repo = new VirtualRepository("some path");
            var joba = new BackupJob("job1", new SplitStorage(), repo);
            joba.AddObject(new VirtualFile("file1.png"));
            joba.AddObject(new VirtualFile("file2.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            joba.DeleteObject(new VirtualFile("file1.png"));
            joba.CreateRestorePoint();
            Assert.AreEqual(joba.PointsCount, 2);
            Assert.AreEqual(repo.StoragesCount(), 3);
        }
        [Test]
        public void SingleVirtualStorageTest()
        {
            var repo = new VirtualRepository("some path");
            var joba = new BackupJob("job1", new SingleStorage(), repo);
            joba.AddObject(new VirtualFile("file1.png"));
            joba.AddObject(new VirtualFile("file2.png"));
            joba.CreateRestorePoint();
            Thread.Sleep(1000);
            joba.DeleteObject(new VirtualFile("file1.png"));
            joba.CreateRestorePoint();
            Assert.AreEqual(joba.PointsCount, 2);
            Assert.AreEqual(repo.StoragesCount(), 2);
        }
    }
}