using System;
using Shops.Services;
using Spectre.Console;

namespace Shops.UI.UserActions
{
    public class ChangePrice : Action
    {
        public ChangePrice(ShopManager manager)
            : base("Change price", manager)
        {
        }

        public override int Execute()
        {
            if (!ValidateHasAssortmentOrShops()) return 0;
            Shop shop = GetUserShopSelection();
            Product product = GetUserProductSelection();
            uint price = AnsiConsole.Ask<uint>("Set new price");
            try
            {
                Manager.ChangePrice(shop, product, price);
            }
            catch (Exception e)
            {
                PrintMessage(e.Message);
            }

            return 0;
        }
    }
}