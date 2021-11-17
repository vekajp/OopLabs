using Banks.BankSystem;

namespace Banks.UI.ClientActions
{
    public abstract class Command
    {
        private string _message;
        public Command(CentralBank centralBank, string message)
        {
            CentralBank = centralBank;
            _message = message;
        }

        protected CentralBank CentralBank { get; }

        public abstract void Execute();
        public override string ToString()
        {
            return _message;
        }
    }
}