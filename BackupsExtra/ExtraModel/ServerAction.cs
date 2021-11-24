namespace BackupsExtra.Restore
{
    public enum ServerAction
    {
        ReceiveRestorePoint,
        DeleteRestorePoint,
        DeleteFileFromRestorePoint,
        AddFileToRestorePoint,
        GetRestorePointsNames,
        GetFileNamesInRestorePoint,
        GetServerFile,
    }
}