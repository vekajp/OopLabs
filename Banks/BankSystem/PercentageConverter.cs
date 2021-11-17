using System;

namespace Banks.BankSystem
{
    public static class PercentageConverter
    {
        public static decimal DailyPercentage(decimal yearlyPercentage)
        {
            decimal daysInYear = DateTime.Now.Year % 4 == 0 ? 366m : 365m;
            return decimal.Round(yearlyPercentage / daysInYear, 2);
        }
    }
}