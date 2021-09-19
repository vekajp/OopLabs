using System;
using System.Collections.Generic;
using System.Linq;

namespace Shops.Services
{
    public class ShopManager
    {
        private List<Shop> _shops;
        private List<Product> _products;
        public ShopManager()
        {
            _shops = new List<Shop>();
            _products = new List<Product>();
        }

        public IReadOnlyList<Shop> Shops => _shops;

        public IReadOnlyList<Product> Products => _products;

        public Shop RegisterShop(int id, string name, string address)
        {
            if (ExistsShopWithId(id)) throw new ArgumentException($"Shop with id \"{id}\" already exists");
            Shop shop = new Shop(id, name, address);
            _shops.Add(shop);
            return shop;
        }

        public Product FindProductById(int id)
        {
            return _products.Find(x => x.Id == id);
        }

        public Product RegisterProduct(int id, string name)
        {
            if (FindProductById(id) != null) throw new Exception($"Product with id \"{id}\" was already registered");
            Product product = new Product(id, name);
            _products.Add(product);
            return product;
        }

        public void ChangePrice(Shop shop, Product product, uint newPrice)
        {
            shop.ChangePrice(product, newPrice);
        }

        public Shop FindCheapestShop(Purchase purchase)
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

        public Shop FindShopById(int id)
        {
            return _shops.Find(shop => shop.Id == id);
        }

        private bool ExistsShopWithId(int id)
        {
            Shop shop = FindShopById(id);
            return shop != null;
        }
    }
}