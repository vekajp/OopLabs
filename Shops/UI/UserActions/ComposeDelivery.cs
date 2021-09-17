using System;
using System.Collections.Generic;
using Shops.Services;
using Spectre.Console;

namespace Shops.UI.UserActions
{
    public class ComposeDelivery : Action
    {
        public ComposeDelivery(ShopManager manager)
            : base("Compose delivery", manager)
        {
        }

        public override int Execute()
        {
            if (!ValidateHasAssortmentOrShops()) return 0;

            List<Product> products = GetUserProductsSelection();
            List<Shop> shops = GetUserShopsSelection();
            Delivery delivery = FormDelivery(products);
            Deliver(delivery, shops);
            return 0;
        }

        private Delivery FormDelivery(List<Product> products)
        {
            var delivery = new Delivery();
            foreach (Product product in products)
            {
                string description = $"{product.Name}({product.Id})";
                uint price = AnsiConsole.Ask<uint>($"Set price for {description}");
                uint quantity = AnsiConsole.Ask<uint>($"Set quantity for {description}");
                delivery.AddProduct(product, new Price(price), quantity);
            }

            return delivery;
        }

        private void Deliver(Delivery delivery, List<Shop> shops)
        {
            foreach (Shop shop in shops)
            {
                delivery.Deliver(shop);
            }
        }
    }
}