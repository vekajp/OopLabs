namespace BackupsExtra.Logging
{
    public interface ILogger
    {
        ILogger SetTimePrefix(bool flag);
        void LogMessage(string message);
    }
}