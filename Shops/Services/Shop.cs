using System;
using System.Collections.Generic;
using System.Linq;

namespace Shops.Services
{
    public class Shop
    {
        public Shop(int id, string name, string address)
        {
            Id = id;
            Name = name;
            Address = address;
            Products = new List<ProductUnit>();
        }

        public int Id { get; }
        public uint Profit { get; private set; }
        public string Name { get; }
        private string Address { get; }
        private List<ProductUnit> Products { get; }

        public ProductUnit FindUnit(Product product)
        {
            return Products.Find(unit => unit.Product == product);
        }

        public void AddProduct(ProductUnit unit)
        {
            ProductUnit shopUnit = FindUnit(unit.Product);
            if (shopUnit != null)
            {
                shopUnit.Merge(unit);
                return;
            }

            Products.Add(unit);
        }

        public Price GetCost(Product product)
        {
            ProductUnit unit = FindUnit(product);
            if (unit == null) throw new Exception($"No {product.Name} in shop {Name}");
            return unit.Price;
        }

        public ProductUnit ChangePrice(Product product, uint newPrice)
        {
            ProductUnit unit = FindUnit(product) ?? throw new ArgumentException($"No product {product.Name} in shop {Name}");
            if (unit.Cost() == newPrice) throw new Exception("New price equals to old price");
            var newUnit = new ProductUnit(unit.Product, new Price(newPrice), unit.Count);
            Products.Remove(unit);
            Products.Add(newUnit);
            return newUnit;
        }

        public void Buy(Person person, Purchase purchase)
        {
            Price price = new Price(purchase.CalculatePriceInShop(this));
            if (!person.IsAbleToPay(price)) throw new Exception("Not enough money to pay");
            foreach (PurchaseUnit item in purchase.ShoppingList)
            {
                ExtractProducts(item);
            }

            person.Buy(price);
            RegisterProfit(price);
        }

        public bool InStock(Purchase purchase)
        {
            return purchase.ShoppingList.All(item => InStock(item));
        }

        private bool InStock(PurchaseUnit item)
        {
            ProductUnit unit = FindUnit(item.Product);
            if (unit == null) return false;
            return unit.Count >= item.Amount;
        }

        private void ExtractProducts(PurchaseUnit item)
        {
            ProductUnit unit = FindUnit(item.Product) ?? throw new ArgumentException();
            unit.ExtractProducts(item.Amount);
        }

        private void RegisterProfit(Price price)
        {
            Profit += price.Value;
        }
    }
}