using Shops.Services;
using Shops.UI.UserActions;
using Spectre.Console;
namespace Shops.UI
{
    public class CommandManager
    {
        private const int OptionsCount = 6;
        public CommandManager()
        {
            Manager = new ShopManager();
            Running = true;
            Client = IntroduceClient();
        }

        public bool Running { get; private set; }
        private ShopManager Manager { get; }
        private Person Client { get; }
        public void Execute()
        {
            string title = $"Client [blue bold]{Client.Name}[/]\nBalance: [blue bold]{Client.Money}[/]";
            Action choice = AnsiConsole.Prompt(
                new SelectionPrompt<Action>()
                    .Title(title)
                    .PageSize(OptionsCount)
                    .AddChoices(new Action[]
                    {
                        new RegisterShop(Manager),
                        new RegisterProduct(Manager),
                        new ComposeDelivery(Manager),
                        new ShopProducts(Manager, Client),
                        new ChangePrice(Manager),
                        new ExitManager(),
                    }));
            if (choice.Execute() == 1) StopRunning();
        }

        private static Person IntroduceClient()
        {
            string name = AnsiConsole.Ask<string>("What's your name?");
            uint money = AnsiConsole.Ask<uint>("How much money do you have?");
            return new Person(name, money);
        }

        private void StopRunning()
        {
            Running = false;
        }
    }
}