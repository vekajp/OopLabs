using Banks.Client;

namespace Banks.UI.ClientActions
{
    public class ExitMainMenuCommand : ClientActionCommand
    {
        public ExitMainMenuCommand()
            : base(null, null, "Exit")
        {
        }

        public override void Execute()
        {
        }

        public BankClient GetClient()
        {
            return null;
        }
    }
}