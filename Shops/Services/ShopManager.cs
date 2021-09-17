using System;
using System.Collections.Generic;
using System.Linq;

namespace Shops.Services
{
    public class ShopManager
    {
        public ShopManager()
        {
            Shops = new List<Shop>();
            Products = new List<Product>();
        }

        public List<Shop> Shops { get; }
        public List<Product> Products { get; }

        public Shop RegisterShop(int id, string name, string address)
        {
            if (ExistsShopWithId(id)) throw new ArgumentException($"Shop with id \"{id}\" already exists");
            Shop shop = new Shop(id, name, address);
            Shops.Add(shop);
            return shop;
        }

        public Product GetProductById(int id)
        {
            return Products.FirstOrDefault(x => x.Id == id);
        }

        public Product RegisterProduct(int id, string name)
        {
            if (GetProductById(id) != null) throw new Exception($"Product with id \"{id}\" was already registered");
            Product product = new Product(id, name);
            Products.Add(product);
            return product;
        }

        public void ChangePrice(Shop shop, Product product, uint newPrice)
        {
            shop.ChangePrice(product, newPrice);
        }

        public Shop GetShopWithCheapestProduct(Product product)
        {
            Shop result = null;
            foreach (Shop shop in Shops.Where(shop => shop.HasProduct(product)))
            {
                result ??= shop;
                result = result.GetCost(product).Value < shop.GetCost(product).Value ? result : shop;
            }

            return result;
        }

        public Shop GetCheapestShop(Purchase purchase)
        {
            Shop cheapest = null;
            Price minimumPrice = new Price(int.MaxValue);
            foreach (Shop shop in Shops.Where(x => x.InStock(purchase)))
            {
                cheapest ??= shop;
                Price currentPrice = new Price(purchase.CalculatePriceInShop(shop));
                if (minimumPrice.Value > currentPrice.Value)
                {
                    cheapest = shop;
                    minimumPrice = currentPrice;
                }
            }

            if (cheapest == null) throw new Exception("Shop not found");
            return cheapest;
        }

        public Shop GetShopById(int id)
        {
            return Shops.FirstOrDefault(shop => shop.Id == id);
        }

        private bool ExistsShopWithId(int id)
        {
            Shop shop = GetShopById(id);
            return shop != null;
        }
    }
}