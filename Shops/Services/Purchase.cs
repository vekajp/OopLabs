using System;
using System.Collections.Generic;
using System.Linq;

namespace Shops.Services
{
    public class Purchase
    {
        private readonly List<PurchaseUnit> _shoppingList;
        public Purchase(List<PurchaseUnit> units)
        {
            _shoppingList = units;
        }

        public IReadOnlyList<PurchaseUnit> ShoppingList => _shoppingList;
        public uint CalculatePriceInShop(Shop shop)
        {
            uint sum = 0;
            foreach (PurchaseUnit item in ShoppingList)
            {
                ProductUnit unit = shop.FindUnit(item.Product);
                if (unit == null) throw new Exception($"Product {item.Product.Name} not found in shop {shop.Name}");
                sum += unit.Cost(item.Amount);
            }

            return sum;
        }

        private PurchaseUnit FindUnit(Product product)
        {
            return _shoppingList.Find(x => x.Product == product);
        }
    }
}