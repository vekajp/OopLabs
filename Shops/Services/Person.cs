using System;

namespace Shops.Services
{
    public class Person
    {
        public Person(string name, uint money)
        {
            Name = name ?? throw new ArgumentNullException();
            Money = money;
        }

        public string Name { get; }
        public uint Money { get; private set; }

        public void Buy(Price price)
        {
            Money -= price.Value;
        }

        public bool IsAbleToPay(Price price)
        {
            return price.Value <= Money;
        }
    }
}