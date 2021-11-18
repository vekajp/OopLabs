using Banks.Accounts;
using Banks.Client;

namespace Banks.BankSystem.AcountBuilding
{
    public class DebitAccountBuilder : AccountBuilder
    {
        public DebitAccountBuilder(Bank bank, BankClient bankClient)
            : base(bank, bankClient)
        {
        }

        public override BankAccount MakeAccount()
        {
            return new DebitAccount(BankClient, Balance, TransactionLimit, DebitYearlyPercent);
        }
    }
}