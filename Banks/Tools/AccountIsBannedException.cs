using System;

namespace Banks.Tools
{
    public class AccountIsBannedException : Exception
    {
        public AccountIsBannedException()
            : base()
        {
        }

        public AccountIsBannedException(string message)
            : base(message)
        {
        }
    }
}