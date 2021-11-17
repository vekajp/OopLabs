using System;
using Banks.Accounts;
using Banks.Tools;

namespace Banks.Transactions
{
    public abstract class Transaction
    {
        public Transaction(decimal amount, BankAccount initiator)
        {
            Amount = amount;
            Success = false;
            Initiator = initiator;
        }

        public Transaction()
        {
        }

        public Guid Id { get; init; }
        public decimal Amount { get; private set; }
        public bool Success { get; protected set; }

        public virtual BankAccount Initiator { get; private set; }
        public abstract bool Complete();
        public virtual void Reverse()
        {
            if (!Success)
            {
                throw new TransactionCannotBeReversedException(nameof(Success));
            }
        }

        public override string ToString()
        {
            return $"{Id}({Amount})";
        }
    }
}