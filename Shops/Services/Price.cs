using System;

namespace Shops.Services
{
    public class Price
    {
        public Price(uint price)
        {
            Value = price;
        }

        public uint Value { get; }
    }
}