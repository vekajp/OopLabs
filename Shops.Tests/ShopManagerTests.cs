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
        
        [SetUp]
        public void SetUp()
        {
            _manager = new ShopManager();
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
            
            Delivery delivery = new Delivery();
            delivery.AddProduct(_manager.GetProductById(1), new Price(100), 10);
            delivery.AddProduct(_manager.GetProductById(2), new Price(200), 10);
            delivery.AddProduct(_manager.GetProductById(3), new Price(300), 10);
            delivery.AddProduct(_manager.GetProductById(4), new Price(400), 10);
            delivery.AddProduct(_manager.GetProductById(5), new Price(500), 10);
            delivery.AddProduct(_manager.GetProductById(6), new Price(600), 10);
            delivery.Deliver(shop);
        }

        [Test]
        public void TestDelivery()
        {
            Shop shop = _manager.GetShopById(1);
            Purchase purchase = new Purchase();
            purchase.AddProduct(_manager.GetProductById(1), 1);

            Assert.That(shop.InStock(purchase));
            
            purchase.AddProduct(_manager.GetProductById(1), 10);
            
            Assert.That(!shop.InStock(purchase));
        }

        [Test]
        public void TestBuyingProducts()
        {
            uint startBudget = 10000;
            Person customer = new Person("vekv", startBudget);
            Purchase purchase = new Purchase();
            purchase.AddProduct(_manager.GetProductById(1), 1);
            purchase.AddProduct(_manager.GetProductById(2), 1);
            purchase.AddProduct(_manager.GetProductById(3), 1);
            purchase.AddProduct(_manager.GetProductById(4), 1);
            purchase.AddProduct(_manager.GetProductById(5), 1);
            purchase.AddProduct(_manager.GetProductById(6), 1);

            Shop shop = _manager.GetShopById(1);
            shop.Buy(customer, purchase);

            uint price = purchase.CalculatePriceInShop(shop);
            uint expectedPrice = purchase.ShoppingList.Aggregate<PurchaseUnit, uint>(0, (current, unit) => current + shop.GetCost(unit.Product).Value);

            Assert.AreEqual(price, expectedPrice);
            Assert.AreEqual(customer.Money, startBudget - price);
            Assert.AreEqual(shop.Profit, price);

            Assert.AreEqual(shop.GetUnit(_manager.GetProductById(1)).Count, 9);

            purchase.Clear();
            
            //test not enough money case
            purchase.AddProduct(_manager.GetProductById(5), 9);
            purchase.AddProduct(_manager.GetProductById(6), 9);

            
            Assert.Catch<Exception>(() =>
            {
                shop.Buy(customer, purchase);
            });

            Assert.AreEqual(customer.Money, startBudget - price);
            Assert.AreEqual(shop.GetUnit(_manager.GetProductById(5)).Count, 9);
            Assert.AreEqual(shop.GetUnit(_manager.GetProductById(6)).Count, 9);
            Assert.AreEqual(shop.Profit, price);

            purchase.Clear();
            
            //test not enough products case
            purchase.AddProduct(_manager.GetProductById(5), 10);
            Assert.Catch<Exception>(() =>
            {
                shop.Buy(customer, purchase);
            });
            Assert.AreEqual(customer.Money, startBudget - price);
            Assert.AreEqual(shop.Profit, price);
            
            //test buying not registered product
            purchase.Clear();
            Product notRegestered = new Product(11, "product");
            purchase.AddProduct(notRegestered, 1);
            Assert.Catch<Exception>(() =>
            {
                shop.Buy(customer, purchase);
            });
            Assert.AreEqual(shop.Profit, price);
        }
        
        [Test]
        public void TestSetAndChangePrices()
        {
            uint oldPrice = 100;
            uint newPrice = 200;
            Product someProduct = new Product(0, "product");
            Shop shop1 = _manager.GetShopById(1);
            Shop shop2 = _manager.GetShopById(2);

            Delivery delivery = new Delivery();
            delivery.AddProduct(someProduct, new Price(oldPrice), 5);
            
            delivery.Deliver(shop1);
            delivery.Deliver(shop2);

            shop1.ChangePrice(someProduct, newPrice);
            
            Assert.AreEqual(shop1.GetUnit(someProduct).Price.Value, newPrice);
            Assert.AreEqual(shop2.GetUnit(someProduct).Price.Value, oldPrice);
            
        }

        [Test]
        public void TestSearchingCheapestShop()
        {
            Shop shop1 = _manager.GetShopById(1);
            Shop shop2 = _manager.GetShopById(2);
            Shop shop3 = _manager.GetShopById(3);

            Delivery delivery = new Delivery();
            delivery.AddProduct(_manager.GetProductById(1), new Price(100), 10);
            delivery.AddProduct(_manager.GetProductById(2), new Price(200), 10);
            delivery.AddProduct(_manager.GetProductById(3), new Price(300), 10);
            
            delivery.Deliver(shop1);
            delivery.Deliver(shop2);
            delivery.Deliver(shop3);

            shop2.ChangePrice(_manager.GetProductById(1), 150);
            shop2.ChangePrice(_manager.GetProductById(2), 100);
            shop2.ChangePrice(_manager.GetProductById(3), 150);

            shop3.ChangePrice(_manager.GetProductById(1), 110);
            shop3.ChangePrice(_manager.GetProductById(2), 120);
            shop3.ChangePrice(_manager.GetProductById(3), 400);

            Purchase purchase = new Purchase();
            purchase.AddProduct(_manager.GetProductById(1), 1);
            purchase.AddProduct(_manager.GetProductById(2), 1);
            purchase.AddProduct(_manager.GetProductById(3), 1);

            Shop shop = _manager.GetCheapestShop(purchase);
            Assert.AreEqual(shop, shop2);

            purchase.AddProduct(_manager.GetProductById(1), 100);

            Assert.Catch<Exception>(() =>
            {
                shop = _manager.GetCheapestShop(purchase);
            });
        }
    }
}