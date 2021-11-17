using System;

namespace Banks.Tools
{
    public class TransactionCannotBeReversedException : Exception
    {
        public TransactionCannotBeReversedException()
            : base()
        {
        }

        public TransactionCannotBeReversedException(string message)
            : base(message)
        {
        }
    }
}