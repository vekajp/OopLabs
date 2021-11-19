using System;
using Banks.BankSystem;
using Banks.Client;

namespace Banks.Accounts
{
    public class DepositAccount : BankAccount
    {
        public DepositAccount(BankClient bankClient, decimal balance, decimal transactionLimit,  DateTime expirationDate, decimal balanceYearlyPercent)
        : base(bankClient, balance, transactionLimit, 0)
        {
            ExpirationDate = expirationDate;
            DailyCoefficient = PercentageConverter.DailyPercentage(balanceYearlyPercent) / 100m;
        }

        public DepositAccount()
        {
        }

        public DepositAccount(DepositAccount other)
        : base(other)
        {
            ExpirationDate = other.ExpirationDate;
            DailyCoefficient = other.DailyCoefficient;
        }

        public DateTime ExpirationDate { get; private set; }
        public decimal DailyCoefficient { get; private set; }
        public override decimal WithdrawCash(decimal amount)
        {
            if (ExpirationDate > DateTime.Now)
                throw new ArgumentException("Cannot cash money from active savings account", nameof(amount));

            return base.WithdrawCash(amount);
        }

        public override void RecountCommission()
        {
            MonthlyCommission += Balance * DailyCoefficient;
        }

        public override BankAccount GetAccountStateByDate(DateTime day)
        {
            if (day < DateTime.Today)
            {
                throw new ArgumentException("Invalid date", nameof(day));
            }

            var followingState = new DepositAccount(this);
            DateTime now = DateTime.Now;
            for (DateTime current = now; current < day; current = current.AddMonths(1))
            {
                for (DateTime currentDays = current; currentDays < current.AddMonths(1); currentDays = currentDays.AddDays(1))
                {
                    followingState.RecountCommission();
                }

                followingState.PayCommission();
            }

            return followingState;
        }
    }
}