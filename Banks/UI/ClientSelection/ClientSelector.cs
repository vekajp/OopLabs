using Banks.BankSystem;
using Banks.Client;
using Command = Banks.UI.ClientActions.Command;

namespace Banks.UI.ClientSelection
{
    public abstract class ClientSelector : Command
    {
        public ClientSelector(CentralBank centralBank, string message)
            : base(centralBank, message)
        {
        }

        public abstract BankClient GetClient();
    }
}