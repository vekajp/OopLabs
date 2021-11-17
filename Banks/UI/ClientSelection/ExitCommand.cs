using Banks.Client;

namespace Banks.UI.ClientSelection
{
    public class ExitCommand : ClientSelector
    {
        public ExitCommand()
            : base(null, "Exit")
        {
        }

        public override void Execute()
        {
        }

        public override BankClient GetClient()
        {
            return null;
        }
    }
}