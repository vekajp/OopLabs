using Banks.Accounts;

namespace Banks.UpdatesSubscribers
{
    public class DepositAccountTermsChange : BankTermsChange
    {
        public DepositAccountTermsChange(string message)
        : base(typeof(DepositAccount), message)
        {
        }
    }
}