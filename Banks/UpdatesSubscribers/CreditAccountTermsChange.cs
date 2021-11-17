using Banks.Accounts;

namespace Banks.UpdatesSubscribers
{
    public class CreditAccountTermsChange : BankTermsChange
    {
        public CreditAccountTermsChange(string message)
        : base(typeof(CreditAccount), message)
        {
        }
    }
}