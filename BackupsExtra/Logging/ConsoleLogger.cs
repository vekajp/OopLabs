using System;
using System.Globalization;

namespace BackupsExtra.Logging
{
    public class ConsoleLogger : ILogger
    {
        private bool _timeFlag;
        public ConsoleLogger()
        {
            _timeFlag = true;
        }

        public ILogger SetTimePrefix(bool flag)
        {
            _timeFlag = flag;
            return this;
        }

        public void LogMessage(string message)
        {
            Console.WriteLine($"{GeneratePrefix()}\t{message}");
        }

        private string GeneratePrefix()
        {
            return _timeFlag ? DateTime.Now.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }
    }
}