using Banks.Accounts;

namespace Banks.Transactions
{
    public class WithdrawalTransaction : Transaction
    {
        public WithdrawalTransaction(BankAccount account, decimal amount)
        : base(amount, account)
        {
            Account = account;
        }

        public WithdrawalTransaction()
        {
        }

        public virtual BankAccount Account { get; init; }

        public override bool Complete()
        {
            Success = Initiator.TryWithdraw(Amount);
            return Success;
        }

        public override void Reverse()
        {
            base.Reverse();
            Account.Deposit(Amount);
            Success = false;
        }
    }
}