namespace BackupsExtra.Logging
{
    public class NullLogger : ILogger
    {
        public ILogger SetTimePrefix(bool flag)
        {
            return this;
        }

        public void LogMessage(string message)
        {
        }
    }
}