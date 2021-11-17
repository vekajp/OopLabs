using Banks.Accounts;

namespace Banks.Transactions
{
    public class P2PTransaction : Transaction
    {
        public P2PTransaction(BankAccount sender, BankAccount receiver, decimal amount)
            : base(amount, sender)
        {
            Receiver = receiver;
        }

        public P2PTransaction()
        {
        }

        public virtual BankAccount Receiver { get; init; }
        public override bool Complete()
        {
            try
            {
                Initiator.Transfer(Receiver, Amount);
                Success = true;
            }
            catch
            {
                Success = false;
            }

            return Success;
        }

        public override void Reverse()
        {
            base.Reverse();
            Initiator.Deposit(Receiver.ForceCash(Amount));
            Success = false;
        }
    }
}