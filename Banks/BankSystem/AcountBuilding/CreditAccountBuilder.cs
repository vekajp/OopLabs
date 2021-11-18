using Banks.Accounts;
using Banks.Client;

namespace Banks.BankSystem.AcountBuilding
{
    public class CreditAccountBuilder : AccountBuilder
    {
        public CreditAccountBuilder(Bank bank, BankClient bankClient)
            : base(bank, bankClient)
        {
        }

        public override BankAccount MakeAccount()
        {
            return new CreditAccount(BankClient, Balance, TransactionLimit, AccountLimit, CreditCommission);
        }
    }
}