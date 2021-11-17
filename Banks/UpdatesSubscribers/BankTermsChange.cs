using System;

namespace Banks.UpdatesSubscribers
{
    public class BankTermsChange
    {
        public BankTermsChange(Type type, string message)
        {
            AccountType = type;
            Message = message;
            Id = Guid.NewGuid();
        }

        public BankTermsChange()
        {
        }

        public Guid Id { get; init; }
        public Type AccountType { get; }
        public string Message { get; }
    }
}