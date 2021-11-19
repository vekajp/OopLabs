using System;
using Banks.Client;
using Banks.Tools;

namespace Banks.Accounts
{
    public abstract class BankAccount
    {
        public BankAccount(BankClient client, decimal initialBalance, decimal transactionLimit, decimal accountLimit)
        {
            if (transactionLimit <= 0)
            {
                throw new ArgumentException("Transaction limit must be positive number", nameof(transactionLimit));
            }

            Client = client ?? throw new ArgumentNullException(nameof(client));
            Balance = initialBalance;
            TransactionLimit = transactionLimit;
            Id = Guid.NewGuid();
            AccountLimit = accountLimit;
            MonthlyCommission = 0;
            Banned = false;
        }

        public BankAccount()
        {
        }

        public BankAccount(BankAccount other)
        {
            Client = other.Client;
            Balance = other.Balance;
            TransactionLimit = other.TransactionLimit;
            Id = other.Id;
            AccountLimit = other.AccountLimit;
            MonthlyCommission = other.MonthlyCommission;
            Banned = other.Banned;
        }

        public decimal Balance { get; protected set; }
        public decimal AccountLimit { get; private set; }
        public decimal TransactionLimit { get; private set; }
        public Guid Id { get; init; }
        public virtual BankClient Client { get; private set; }

        public bool Banned { get; private set; }
        protected decimal MonthlyCommission { get; set; }

        public void Deposit(decimal amount)
        {
            if (Banned)
                throw new AccountIsBannedException();

            if (amount < 0)
                throw new ArgumentException("Cannot deposit negative amount", nameof(amount));

            Balance += amount;
        }

        public bool TryDeposit(decimal amount)
        {
            try
            {
                Deposit(amount);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual decimal WithdrawCash(decimal amount)
        {
            if (Banned)
                throw new AccountIsBannedException();

            if (amount < 0)
                throw new ArgumentException("Cannot cash negative amount", nameof(amount));

            if (!Client.TrustWorthy && amount > TransactionLimit)
                throw new ArgumentException("Transaction limit was exceeded", nameof(amount));

            if (Balance - amount < AccountLimit)
                throw new ArgumentException("Account limit was exceeded", nameof(amount));

            Balance -= amount;
            return amount;
        }

        public bool Transfer(BankAccount receiver, decimal amount)
        {
            if (!TryWithdraw(amount)) return false;
            if (receiver.TryDeposit(amount)) return true;
            Deposit(amount);
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var account = (BankAccount)obj;
            return Id == account.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public decimal ForceCash(decimal amount)
        {
            if (TryWithdraw(amount)) return amount;
            Balance -= amount;
            Banned = true;
            return amount;
        }

        public void PayCommission()
        {
            Balance += MonthlyCommission;
            MonthlyCommission = 0;
        }

        public abstract void RecountCommission();

        public abstract BankAccount GetAccountStateByDate(DateTime day);

        public override string ToString()
        {
            return $"{Id.ToString()}, ({Client})";
        }

        public bool TryWithdraw(decimal amount)
        {
            try
            {
                WithdrawCash(amount);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}