using Banks.BankSystem;
using Banks.Client;

namespace Banks.UI.ClientActions
{
    public class ClientActionCommand : Command
    {
        public ClientActionCommand(CentralBank centralBank, BankClient client, string message)
            : base(centralBank, message)
        {
            Client = client;
        }

        protected BankClient Client { get; set; }

        public override void Execute()
        {
        }
    }
}