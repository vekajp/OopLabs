using Banks.Accounts;
using Banks.Client;

namespace Banks.BankSystem.AcountBuilding
{
    public class DepositAccountBuilder : AccountBuilder
    {
        public DepositAccountBuilder(Bank bank, BankClient bankClient)
            : base(bank, bankClient)
        {
        }

        public override BankAccount MakeAccount()
        {
            return new DepositAccount(BankClient, Balance, TransactionLimit, ExpirationDate, DepositYearlyPercent);
        }
    }
}