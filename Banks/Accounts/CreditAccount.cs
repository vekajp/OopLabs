using System;
using Banks.Client;

namespace Banks.Accounts
{
    public class CreditAccount : BankAccount
    {
        public CreditAccount(BankClient bankClient, decimal initialBalance, decimal transactionLimit, decimal accountLimit, decimal usageCommission)
        : base(bankClient, initialBalance, transactionLimit, accountLimit)
        {
            if (accountLimit > 0)
            {
                throw new ArgumentException("Credit account limit must be negative number", nameof(accountLimit));
            }

            if (usageCommission <= 0)
            {
                throw new ArgumentException("Commission must be positive number", nameof(usageCommission));
            }

            UsageCommission = usageCommission;
        }

        public CreditAccount()
        {
        }

        private CreditAccount(CreditAccount other)
        : base(other)
        {
            UsageCommission = other.UsageCommission;
        }

        public decimal UsageCommission { get; private set; }
        public override decimal Cash(decimal amount)
        {
            base.Cash(amount);
            RecountCommission();
            return amount;
        }

        public override void RecountCommission()
        {
            if (Balance < 0)
            {
                MonthlyCommission -= UsageCommission;
            }
        }

        public override BankAccount GetAccountStateByDate(DateTime day)
        {
            if (day < DateTime.Today)
            {
                throw new ArgumentException("Invalid date", nameof(day));
            }

            return this;
        }
    }
}