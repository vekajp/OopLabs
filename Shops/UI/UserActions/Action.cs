using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Shops.Services;
using Spectre.Console;

namespace Shops.UI.UserActions
{
    public abstract class Action
    {
        protected Action(string action, ShopManager manager)
        {
            ActionName = action;
            Manager = manager;
        }

        protected ShopManager Manager { get; }
        private string ActionName { get; }
        public abstract int Execute();

        public override string ToString()
        {
            return ActionName;
        }

        protected static void PrintMessage(string message)
        {
            AnsiConsole.Markup($"[red bold]Error: [/]{message}\n");
        }

        protected bool ValidateHasAssortmentOrShops()
        {
            if (Manager.Shops.Count == 0)
            {
                PrintMessage("No shops registered yet");
                return false;
            }

            if (Manager.Products.Count != 0) return true;
            PrintMessage("No products registered yet");
            return false;
        }

        protected Shop GetUserShopSelection()
        {
            List<string> shops = GetShopNames();
            string shopChoice = GetUserSingleChoise(shops);
            return Manager.FindShopById(GetId(shopChoice));
        }

        protected Product GetUserProductSelection()
        {
            List<string> products = GetProductNames();
            string shopChoice = GetUserSingleChoise(products);
            return Manager.FindProductById(GetId(shopChoice));
        }

        protected List<Product> GetUserProductsSelection()
        {
            List<string> productNames = GetUserMultipleChoice("Select products", GetProductNames());
            return productNames.Select(name => Manager.FindProductById(GetId(name))).ToList();
        }

        protected List<Shop> GetUserShopsSelection()
        {
            List<string> shopNames = GetUserMultipleChoice("Select shops", GetShopNames());
            return shopNames.Select(name => Manager.FindShopById(GetId(name))).ToList();
        }

        private static string FormatNameAndId(string name, int id)
        {
            return $"{name}({id})";
        }

        private static int GetId(string formatedString)
        {
            int id = int.Parse(Regex.Match(formatedString, @"\(([^)]*)\)").Groups[1].Value);
            return id;
        }

        private static List<string> GetUserMultipleChoice(string title, List<string> options)
        {
            int pageSize = options.Count > 3 ? options.Count : 3;
            List<string> chosenOptions = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title(title)
                    .PageSize(pageSize)
                    .AddChoices(options));
            return chosenOptions;
        }

        private static string GetUserSingleChoise(List<string> choices)
        {
            int optionsCount = choices.Count > 3 ? choices.Count : 3;
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose one of following options")
                    .PageSize(optionsCount)
                    .AddChoices(choices));
            return choice;
        }

        private List<string> GetShopNames()
        {
            return Manager.Shops.Select(shop => FormatNameAndId(shop.Name, shop.Id)).ToList();
        }

        private List<string> GetProductNames()
        {
            return Manager.Products.Select(product => FormatNameAndId(product.Name, product.Id)).ToList();
        }
    }
}