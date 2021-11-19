using System;
using Banks.BankSystem;
using Banks.Client;

namespace Banks.Accounts
{
    public class DebitAccount : BankAccount
    {
        public DebitAccount(BankClient bankClient, decimal balance, decimal transactionLimit, decimal balanceYearlyPercent)
        : base(bankClient, balance, transactionLimit, 0)
        {
            DailyCoefficient = PercentageConverter.DailyPercentage(balanceYearlyPercent) / 100m;
        }

        public DebitAccount()
        {
        }

        private DebitAccount(DebitAccount other)
        : base(other)
        {
            DailyCoefficient = other.DailyCoefficient;
        }

        public decimal DailyCoefficient { get; private set; }
        public override void RecountCommission()
        {
            MonthlyCommission += Balance * DailyCoefficient;
        }

        public override BankAccount GetAccountStateByDate(DateTime day)
        {
            if (day < DateTime.Today)
                throw new ArgumentException("Invalid date", nameof(day));

            var followingState = new DebitAccount(this);
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