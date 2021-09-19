using System;

namespace Shops.Services
{
    public class PurchaseUnit
    {
        public PurchaseUnit(Product product, uint amount)
        {
            Product = product ?? throw new ArgumentNullException(nameof(product));
            Amount = amount;
        }

        public Product Product { get; }
        public uint Amount { get; private set; }

        public void Merge(PurchaseUnit unit)
        {
            if (unit == this) return;
            Amount += unit.Amount;
        }
    }
}