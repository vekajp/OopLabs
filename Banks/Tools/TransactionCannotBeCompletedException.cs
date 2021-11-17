using System;

namespace Banks.Tools
{
    public class TransactionCannotBeCompletedException : Exception
    {
        public TransactionCannotBeCompletedException()
            : base()
        {
        }

        public TransactionCannotBeCompletedException(string message)
            : base(message)
        {
        }
    }
}