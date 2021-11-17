using Banks.BankSystem;
using Banks.Client;
using Banks.UI.ClientActions;
using Banks.UI.ClientSelection;
using Spectre.Console;

namespace Banks.UI
{
    public class UserInterface
    {
        private const int OptionsCount = 5;
        private bool _running;
        private bool _definingClient;
        private CentralBank _centralBank;
        private BankClient _currectClient;
        public UserInterface(CentralBank centralBank)
        {
            _centralBank = centralBank;
            _running = true;
            _definingClient = true;
        }

        public void Run()
        {
            while (_running || _definingClient)
            {
                if (_definingClient)
                {
                    _currectClient = DefineClient();
                    _definingClient = false;
                }

                if (_running)
                {
                    RunClientActionsMenu();
                }
            }
        }

        private void RunClientActionsMenu()
        {
            Command choice = AnsiConsole.Prompt(new SelectionPrompt<ClientActionCommand>()
                .Title($"Client {_currectClient}")
                .PageSize(OptionsCount)
                .AddChoices(new ClientActionCommand[]
                {
                    new OpenAccount(_centralBank, _currectClient),
                    new MakeTransaction(_centralBank, _currectClient),
                    new CancelTransaction(_centralBank, _currectClient),
                    new ExitMainMenuCommand(),
                }));

            choice.Execute();
            if (choice.GetType() != typeof(ExitMainMenuCommand)) return;
            AnsiConsole.Clear();
            _definingClient = true;
            _running = false;
        }

        private BankClient DefineClient()
        {
            ClientSelector choice = AnsiConsole.Prompt(new SelectionPrompt<ClientSelector>()
                .Title("Main menu")
                .PageSize(OptionsCount)
                .AddChoices(new ClientSelector[]
                {
                    new RegisterClient(_centralBank),
                    new SelectClient(_centralBank),
                    new ExitCommand(),
                }));
            _running = true;
            if (choice.GetType() == typeof(ExitCommand))
            {
                _running = false;
            }

            return choice.GetClient();
        }
    }
}