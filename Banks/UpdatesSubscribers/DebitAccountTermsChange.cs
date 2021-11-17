using Banks.Accounts;

namespace Banks.UpdatesSubscribers
{
    public class DebitAccountTermsChange : BankTermsChange
    {
        public DebitAccountTermsChange(string message)
            : base(typeof(DebitAccount), message)
        {
        }
    }
}