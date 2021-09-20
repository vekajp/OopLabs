using System;

namespace Shops.Services
{
    public class ProductUnit
    {
        public ProductUnit(Product product, Price price, uint count)
        {
            Product = product ?? throw new ArgumentNullException(nameof(product));
            Count = count;
            Price = price;
        }

        public Product Product { get; }
        public uint Count { get; private set; }
        public Price Price { get; private set; }

        public uint Cost(uint number = 1)
        {
            return number * Price.Value;
        }

        public void ExtractProducts(uint number)
        {
            if (Count < number) throw new ArgumentOutOfRangeException(nameof(number), "Not enough products");
            Count -= number;
        }

        public void Merge(ProductUnit unit)
        {
            if (unit == this) return;
            Price = Price.Value < unit.Price.Value ? Price : unit.Price;
            AddProducts(unit.Count);
        }

        private void AddProducts(uint number)
        {
            Count += number;
        }
    }
}