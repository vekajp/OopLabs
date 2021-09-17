using System;
using Shops.Services;
using Spectre.Console;

namespace Shops.UI.UserActions
{
    public class RegisterShop : Action
    {
        public RegisterShop(ShopManager manager)
            : base("Register shop", manager)
        {
        }

        public override int Execute()
        {
            int id = AnsiConsole.Ask<int>("Enter shop identifier");
            string name = AnsiConsole.Ask<string>("Shop name?");
            string address = AnsiConsole.Ask<string>("Address?");
            try
            {
                Manager.RegisterShop(id, name, address);
            }
            catch (Exception e)
            {
                PrintMessage(e.Message);
            }

            return 0;
        }
    }
}