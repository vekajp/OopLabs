using System.Collections.Generic;
using Banks.BankSystem;
using Banks.Client;
using Spectre.Console;

namespace Banks.UI.ClientSelection
{
    public class SelectClient : ClientSelector
    {
        private BankClient _client;
        public SelectClient(CentralBank centralBank)
            : base(centralBank, "Select existing client")
        {
        }

        public override void Execute()
        {
            IReadOnlyCollection<BankClient> clients = CentralBank.Clients;
            int countClients = clients.Count;
            if (countClients == 0)
            {
                AnsiConsole.Markup("No accounts yet");
                return;
            }

            BankClient client = AnsiConsole.Prompt(
                new SelectionPrompt<BankClient>()
                    .Title("Select client")
                    .PageSize(countClients > 3 ? countClients : 3)
                    .AddChoices(clients));
            _client = client;
        }

        public override BankClient GetClient()
        {
            Execute();
            return _client;
        }
    }
}