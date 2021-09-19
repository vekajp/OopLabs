using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shops.Services;
namespace Shops.Tests
{
    public class ShopManagerTests
    {
        private ShopManager _manager;
        private Person _customer;
        private uint _startBudget = 10000;
        
        [SetUp]
        public void SetUp()
        {
            _manager = new ShopManager();
            _customer = new Person("vkek", _startBudget);
            Shop shop = _manager.RegisterShop(1, "shop1", "str1");
            _manager.RegisterShop(2, "shop2", "str2a");
            _manager.RegisterShop(3, "shop3", "str3");
            
            _manager.RegisterProduct(0, "product0");
            _manager.RegisterProduct(1, "product1");
            _manager.RegisterProduct(2, "product2");
            _manager.RegisterProduct(3, "product3");
            _manager.RegisterProduct(4, "product4");
            _manager.RegisterProduct(5, "product5");
            _manager.RegisterProduct(6, "product6");
            
            Delivery delivery = new Delivery( new List<ProductUnit>()
            {
                new ProductUnit(_manager.FindProductById(1), new Price(100), 10),
                new ProductUnit(_manager.FindProductById(2), new Price(200), 10),
                new ProductUnit(_manager.FindProductById(3), new Price(300), 10),
                new ProductUnit(_manager.FindProductById(4), new Price(400), 10),
                new ProductUnit(_manager.FindProductById(5), new Price(500), 10),
                new ProductUnit(_manager.FindProductById(6), new Price(600), 10),
            });
            
            delivery.Deliver(shop);
        }

        [Test]
        public void TestDelivery()
        {
            Shop shop = _manager.FindShopById(1);

            Purchase purchase = new Purchase(new List<PurchaseUnit>()
            {
                new PurchaseUnit(_manager.FindProductById(1), 1),
            });
            Assert.That(shop.InStock(purchase));
            
            purchase = new Purchase(new List<PurchaseUnit>()
            {
                new PurchaseUnit(_manager.FindProductById(1), 11),
            });
            Assert.That(!shop.InStock(purchase));
        }

        [Test]
        public void TestBuyingProducts()
        {
            Purchase purchase = new Purchase(new List<PurchaseUnit>()
            {
                new PurchaseUnit(_manager.FindProductById(1), 1),
                new PurchaseUnit(_manager.FindProductById(2), 1),
                new PurchaseUnit(_manager.FindProductById(3), 1),
                new PurchaseUnit(_manager.FindProductById(4), 1),
                new PurchaseUnit(_manager.FindProductById(5), 1),
                new PurchaseUnit(_manager.FindProductById(6), 1),
            });

            Shop shop = _manager.FindShopById(1);
            shop.Buy(_customer, purchase);
            
            uint price = purchase.CalculatePriceInShop(shop);
            uint expectedPrice = purchase.ShoppingList.Aggregate<PurchaseUnit, uint>(0, (current, unit) => current + shop.GetCost(unit.Product).Value);

            Assert.AreEqual(price, expectedPrice);
            Assert.AreEqual(_customer.Money, _startBudget - price);
            Assert.AreEqual(shop.Profit, price);

            Assert.AreEqual(shop.FindUnit(_manager.FindProductById(1)).Count, 9);
            
        }

        [Test]
        public void TestNotEnoughMoney()
        {
            Shop shop = _manager.FindShopById(1);
            uint startProfit = shop.Profit;
            Purchase purchase = new Purchase(new List<PurchaseUnit>()
            {
                new PurchaseUnit(_manager.FindProductById(5), 10),
                new PurchaseUnit(_manager.FindProductById(6), 10)
            });


            Assert.Catch<Exception>(() =>
            {
                shop.Buy(_customer, purchase);
            });

            Assert.AreEqual(_customer.Money, _startBudget);
            Assert.AreEqual(shop.FindUnit(_manager.FindProductById(5)).Count, 10);
            Assert.AreEqual(shop.FindUnit(_manager.FindProductById(6)).Count, 10);
            Assert.AreEqual(shop.Profit, startProfit);
        }

        [Test]
        public void TestNotEnoughProducts()
        {
            Shop shop = _manager.FindShopById(1);
            Purchase purchase = new Purchase(new List<PurchaseUnit>()
            {
                new PurchaseUnit(_manager.FindProductById(5), 11)
            });
            Assert.Catch<Exception>(() =>
            {
                shop.Buy(_customer, purchase);
            });
            Assert.AreEqual(_customer.Money, _startBudget);
            Assert.AreEqual(shop.Profit, 0);
        }

        [Test]
        public void TestBuyUnregisteredProduct()
        {
            Shop shop = _manager.FindShopById(1);
            uint startShopProfit = shop.Profit;
            Product notRegestered = new Product(11, "product");
            Purchase purchase = new Purchase(new List<PurchaseUnit>()
            {
                new PurchaseUnit(notRegestered, 1),
            });
            Assert.Catch<Exception>(() =>
            {
                shop.Buy(_customer, purchase);
            });
            Assert.AreEqual(shop.Profit, startShopProfit);
        }
        
        [Test]
        public void TestSetAndChangePrices()
        {
            uint oldPrice = 100;
            uint newPrice = 200;
            Product someProduct = new Product(0, "product");
            Shop shop1 = _manager.FindShopById(1);
            Shop shop2 = _manager.FindShopById(2);

            Delivery delivery = new Delivery(new List<ProductUnit>()
            {
                new ProductUnit(someProduct, new Price(oldPrice), 5)
            });

            delivery.Deliver(shop1);
            delivery.Deliver(shop2);

            shop1.ChangePrice(someProduct, newPrice);
            
            Assert.AreEqual(shop1.FindUnit(someProduct).Price.Value, newPrice);
            Assert.AreEqual(shop2.FindUnit(someProduct).Price.Value, oldPrice);
            
        }

        [Test]
        public void TestSearchingCheapestShop()
        {
            Shop shop1 = _manager.FindShopById(1);
            Shop shop2 = _manager.FindShopById(2);
            Shop shop3 = _manager.FindShopById(3);

            Delivery delivery = new Delivery(new List<ProductUnit>()
            {
                new ProductUnit(_manager.FindProductById(1), new Price(100), 10),
                new ProductUnit(_manager.FindProductById(2), new Price(200), 10),
                new ProductUnit(_manager.FindProductById(3), new Price(300), 10)
            });
           
            delivery.Deliver(shop1);
            delivery.Deliver(shop2);
            delivery.Deliver(shop3);

            shop2.ChangePrice(_manager.FindProductById(1), 150);
            shop2.ChangePrice(_manager.FindProductById(2), 100);
            shop2.ChangePrice(_manager.FindProductById(3), 150);

            shop3.ChangePrice(_manager.FindProductById(1), 110);
            shop3.ChangePrice(_manager.FindProductById(2), 120);
            shop3.ChangePrice(_manager.FindProductById(3), 400);

            Purchase purchase = new Purchase(new List<PurchaseUnit>()
            {
                new PurchaseUnit(_manager.FindProductById(1), 1),
                new PurchaseUnit(_manager.FindProductById(2), 1),
                new PurchaseUnit(_manager.FindProductById(3), 1)

            });
            Shop shop = _manager.FindCheapestShop(purchase);
            Assert.AreEqual(shop, shop2);

            purchase = new Purchase(new List<PurchaseUnit>()
            {
                new PurchaseUnit(_manager.FindProductById(1), 100),
            });
            Assert.Catch<Exception>(() =>
            {
                shop = _manager.FindCheapestShop(purchase);
            });
        }
    }
}