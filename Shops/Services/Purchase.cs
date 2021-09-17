using System;
using System.Collections.Generic;
using System.Linq;

namespace Shops.Services
{
    public class Purchase
    {
        public Purchase()
        {
            ShoppingList = new List<PurchaseUnit>();
        }

        public List<PurchaseUnit> ShoppingList { get; }

        public PurchaseUnit AddProduct(Product product, uint number)
        {
            PurchaseUnit unit = GetUnit(product);
            if (unit != null)
            {
                return unit.Merge(new PurchaseUnit(product, number));
            }

            unit = new PurchaseUnit(product, number);
            ShoppingList.Add(unit);
            return unit;
        }

        public uint CalculatePriceInShop(Shop shop)
        {
            uint sum = 0;
            foreach (PurchaseUnit item in ShoppingList)
            {
                ProductUnit unit = shop.GetUnit(item.Product);
                if (unit == null) throw new Exception($"Product {item.Product.Name} not found in shop {shop.Name}");
                sum += unit.Cost(item.Amount);
            }

            return sum;
        }

        public void Clear()
        {
            ShoppingList.Clear();
        }

        private PurchaseUnit GetUnit(Product product)
        {
            return ShoppingList.FirstOrDefault(x => x.Product == product);
        }
    }
}