using System;
using System.Collections.Generic;
using Shops.Services;
using Spectre.Console;

namespace Shops.UI.UserActions
{
    public class ShopProducts : Action
    {
        public ShopProducts(ShopManager manager, Person client)
            : base("Shop products", manager)
        {
            Client = client;
        }

        private Person Client { get; }
        public override int Execute()
        {
            if (!ValidateHasAssortmentOrShops()) return 0;

            List<Product> products = GetUserProductsSelection();

            Purchase purchase = FormPurchase(products);

            try
            {
                Shop shop = GetTargetShop(purchase);
                Buy(shop, purchase);
            }
            catch (Exception e)
            {
                PrintMessage(e.Message);
            }

            return 0;
        }

        private static Purchase FormPurchase(List<Product> products)
        {
            var units = new List<PurchaseUnit>();
            foreach (Product product in products)
            {
                string description = $"{product.Name}({product.Id})";
                uint amount = AnsiConsole.Ask<uint>($"Enter amount for product {description}");
                units.Add(new PurchaseUnit(product, amount));
            }

            return new Purchase(units);
        }

        private Shop GetTargetShop(Purchase purchase)
        {
            Shop shop = null;
            shop = AnsiConsole.Confirm("Choose the cheapest shop?") ? Manager.FindCheapestShop(purchase) : GetUserShopSelection();

            return shop;
        }

        private void Buy(Shop shop, Purchase purchase)
        {
            if (shop == null)
            {
                PrintMessage("Shop with suitable assortment not found");
                return;
            }

            try
            {
                shop.Buy(Client, purchase);
            }
            catch (Exception e)
            {
                PrintMessage(e.Message);
            }
        }
    }
}