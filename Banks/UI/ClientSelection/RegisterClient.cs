using Banks.BankSystem;
using Banks.Client;
using Spectre.Console;

namespace Banks.UI.ClientSelection
{
    public class RegisterClient : ClientSelector
    {
        private BankClient _client;
        public RegisterClient(CentralBank bank)
        : base(bank, "Register client")
        {
        }

        public override void Execute()
        {
            string name = AnsiConsole.Ask<string>("Enter client's name");
            string surname = AnsiConsole.Ask<string>("Enter client's surname");
            var client = new BankClient(name, surname);

            if (AnsiConsole.Confirm("Set address?"))
            {
                client.SetAddress(AnsiConsole.Ask<string>("Enter client's address"));
            }

            if (AnsiConsole.Confirm("Set passport data?"))
            {
                client.SetAddress(AnsiConsole.Ask<string>("Enter client's passport number"));
            }

            CentralBank.RegisterClient(client);
            _client = client;
        }

        public override BankClient GetClient()
        {
            Execute();
            return _client;
        }

        public override string ToString()
        {
            return "Register client";
        }
    }
}