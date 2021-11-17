using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Banks.BankSystem
{
    public class BankAccountTerms
    {
        private List<PercentageGap> _depositPercentageGaps;
        public BankAccountTerms(decimal defaultYearlyDepositPercentage, decimal debitAccountYearlyPercentage, decimal creditAccountCommission, decimal transactionLimit, decimal creditAccountLimit)
        {
            _depositPercentageGaps = new List<PercentageGap>();
            ChangeDefaultYearlyDepositPercentage(defaultYearlyDepositPercentage);
            ChangeDebitAccountYearlyPercentage(debitAccountYearlyPercentage);
            ChangeCreditAccountCommission(creditAccountCommission);
            ChangeTransactionLimit(transactionLimit);
            ChangeCreditAccountLimit(creditAccountLimit);
            Id = Guid.NewGuid();
        }

        public BankAccountTerms()
        {
            _depositPercentageGaps = new List<PercentageGap>();
        }

        public Guid Id { get; private set; }

        public decimal DefaultYearlyDepositPercentage { get; private set; }
        public decimal DebitAccountYearlyPercentage { get; private set; }
        public decimal CreditAccountCommission { get; private set; }

        public decimal TransactionLimit { get; private set; }

        public decimal CreditAccountLimit { get; private set; }

        [BackingField(nameof(_depositPercentageGaps))]
        public virtual IReadOnlyCollection<PercentageGap> DepositGaps => _depositPercentageGaps;

        public void AddGap(decimal lowerBound, decimal value)
        {
            var gap = new PercentageGap(lowerBound, value);
            AddGap(gap);
        }

        public void AddGap(PercentageGap gap)
        {
            if (_depositPercentageGaps.Contains(gap))
            {
                _depositPercentageGaps.First(x => x.Equals(gap)).ChangePercentage(gap.YearlyPercent);
            }

            int index = _depositPercentageGaps.IndexOf(_depositPercentageGaps.Find(x => x > gap));
            if (index == -1)
            {
                _depositPercentageGaps.Add(gap);
            }
            else
            {
                _depositPercentageGaps.Insert(index, gap);
            }
        }

        public decimal GetDepositPercentage(decimal balance)
        {
            PercentageGap target = DepositGaps.LastOrDefault(x => balance >= x);
            return target?.YearlyPercent ?? DefaultYearlyDepositPercentage;
        }

        public void ChangeDefaultYearlyDepositPercentage(decimal value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Percentage should be a positive number", nameof(value));
            }

            DefaultYearlyDepositPercentage = value;
        }

        public void ChangeCreditAccountCommission(decimal value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Commission should be a positive number", nameof(value));
            }

            CreditAccountCommission = value;
        }

        public void ChangeDebitAccountYearlyPercentage(decimal value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Percent should be a positive number", nameof(value));
            }

            DebitAccountYearlyPercentage = value;
        }

        public void ChangeTransactionLimit(decimal value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Transaction limit should be a positive number", nameof(value));
            }

            TransactionLimit = value;
        }

        public void ChangeCreditAccountLimit(decimal value)
        {
            if (value > 0)
            {
                throw new ArgumentException("Credit account limit should be a negative number", nameof(value));
            }

            CreditAccountLimit = value;
        }
    }
}