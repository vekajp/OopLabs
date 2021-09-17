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

        public ProductUnit GetUnit(string productName)
        {
            return Products.FirstOrDefault(unit => unit.Product.Name == productName);
        }

        public ProductUnit GetUnit(Product product)
        {
            return Products.FirstOrDefault(unit => unit.Product == product);
        }

        public ProductUnit AddProduct(ProductUnit unit)
        {
            ProductUnit shopUnit = GetUnit(unit.Product);
            if (shopUnit != null)
            {
                return shopUnit.Merge(unit);
            }

            Products.Add(unit);
            return unit;
        }

        public bool HasProduct(Product product)
        {
            return GetUnit(product) != null;
        }

        public Price GetCost(Product product)
        {
            ProductUnit unit = GetUnit(product);
            if (unit == null) throw new Exception($"No {product.Name} in shop {Name}");
            return unit.Price;
        }

        public ProductUnit ChangePrice(Product product, uint newPrice)
        {
            ProductUnit unit = GetUnit(product) ?? throw new ArgumentException($"No product {product.Name} in shop {Name}");
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
            ProductUnit unit = GetUnit(item.Product);
            return unit.Count >= item.Amount;
        }

        private void ExtractProducts(PurchaseUnit item)
        {
            ProductUnit unit = GetUnit(item.Product) ?? throw new ArgumentException();
            unit.ExtractProducts(item.Amount);
        }

        private void RegisterProfit(Price price)
        {
            Profit += price.Value;
        }
    }
}