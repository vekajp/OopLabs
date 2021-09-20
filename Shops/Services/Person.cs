using System;

namespace Shops.Services
{
    public class Person
    {
        public Person(string name, uint money)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Money = money;
        }

        public string Name { get; }
        public uint Money { get; private set; }

        public void Buy(Price price)
        {
            if (!IsAbleToPay(price)) throw new ArgumentException(nameof(price));
            Money -= price.Value;
        }

        public bool IsAbleToPay(Price price)
        {
            return price.Value <= Money;
        }
    }
}