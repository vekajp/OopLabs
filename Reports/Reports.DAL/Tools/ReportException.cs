using System;

namespace Reports.DAL.Tools
{
    public class ReportException : Exception
    {
        public ReportException(string message)
            : base(message)
        {
        }
    }
}