using System;
using System.Text.RegularExpressions;

namespace Banks.Client
{
    public class PassportNumber
    {
        public PassportNumber(string number)
        {
            _ = number ?? throw new ArgumentNullException(nameof(number));
            if (!Regex.IsMatch(number, "[0-9]{10}"))
            {
                throw new ArgumentException("Invalid passport number", nameof(number));
            }

            Number = number;
        }

        public PassportNumber()
        {
        }

        public Guid Id { get; private set; }
        public string Number { get; private set; }
    }
}