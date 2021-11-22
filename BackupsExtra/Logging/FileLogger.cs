using System;
using System.Globalization;
using System.IO;

namespace BackupsExtra.Logging
{
    public class FileLogger : ILogger
    {
        private bool _timeFlag;
        private string _path;
        public FileLogger(string path)
        {
            _timeFlag = true;
            _path = path;
        }

        public ILogger SetTimePrefix(bool flag)
        {
            _timeFlag = flag;
            return this;
        }

        public void LogMessage(string message)
        {
            using StreamWriter writer = File.AppendText(_path);
            writer.WriteLine($"{GeneratePrefix()}\t{message}");
        }

        private string GeneratePrefix()
        {
            return _timeFlag ? DateTime.Now.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }
    }
}