using System;

namespace Backups.BackupAbstractModel
{
    public static class Storage
    {
        public static IStorage GetStorage(StorageType type, RestorePoint point)
        {
            return type switch
            {
                StorageType.SingleType => new SingleStorage(point),
                StorageType.SplitType => new SplitStorage(point),
                _ => throw new ArgumentException("Invalid storage type", nameof(type))
            };
        }
    }
}