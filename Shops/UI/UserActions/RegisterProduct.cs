using System;
using Shops.Services;
using Spectre.Console;

namespace Shops.UI.UserActions
{
    public class RegisterProduct : Action
    {
        public RegisterProduct(ShopManager manager)
            : base("Register product", manager)
        {
        }

        public override int Execute()
        {
            int id = AnsiConsole.Ask<int>("Product identifier?");
            string name = AnsiConsole.Ask<string>("Product name?");
            try
            {
                Manager.RegisterProduct(id, name);
            }
            catch (Exception e)
            {
               PrintMessage(e.Message);
            }

            return 0;
        }
    }
}