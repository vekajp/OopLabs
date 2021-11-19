using Banks.Accounts;

namespace Banks.Transactions
{
    public class DepositTransaction : Transaction
    {
        public DepositTransaction(BankAccount account, decimal amount)
        : base(amount, account)
        {
        }

        public DepositTransaction()
        {
        }

        public override bool Complete()
        {
            Success = Initiator.TryDeposit(Amount);
            return Success;
        }

        public override void Reverse()
        {
            base.Reverse();
            Initiator.ForceCash(Amount);
            Success = false;
        }
    }
}