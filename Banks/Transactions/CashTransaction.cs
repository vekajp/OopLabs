using Banks.Accounts;

namespace Banks.Transactions
{
    public class CashTransaction : Transaction
    {
        public CashTransaction(BankAccount account, decimal amount)
        : base(amount, account)
        {
            Account = account;
        }

        public CashTransaction()
        {
        }

        public virtual BankAccount Account { get; init; }
        public override bool Complete()
        {
            try
            {
                Account.Cash(Amount);
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
            Account.Deposit(Amount);
            Success = false;
        }
    }
}