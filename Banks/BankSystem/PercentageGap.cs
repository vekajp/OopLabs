using System;

namespace Banks.BankSystem
{
    public class PercentageGap
    {
        private const decimal Tolerance = 0.01m;
        public PercentageGap(decimal lowerBound, decimal yearlyPercent)
        {
            Id = Guid.NewGuid();
            LowerBound = lowerBound;
            YearlyPercent = yearlyPercent;
        }

        public PercentageGap()
        {
        }

        public Guid Id { get; set; }
        public decimal LowerBound { get; set; }
        public decimal YearlyPercent { get; private set; }

        public static bool operator <(PercentageGap lhs, PercentageGap rhs)
        {
            return lhs.LowerBound < rhs.LowerBound;
        }

        public static bool operator >(PercentageGap lhs, PercentageGap rhs)
        {
            return lhs.LowerBound > rhs.LowerBound;
        }

        public static bool operator <=(decimal balance, PercentageGap rhs)
        {
            return balance <= rhs.LowerBound;
        }

        public static bool operator >=(decimal balance, PercentageGap rhs)
        {
            return balance >= rhs.LowerBound;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;
            var other = (PercentageGap)obj;
            return Math.Abs(other.LowerBound - LowerBound) < Tolerance;
        }

        public override int GetHashCode()
        {
            return LowerBound.GetHashCode();
        }

        public void ChangePercentage(decimal value)
        {
            YearlyPercent = value;
        }
    }
}