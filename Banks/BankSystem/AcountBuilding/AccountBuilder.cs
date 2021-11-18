using System;
using Banks.Accounts;
using Banks.Client;

namespace Banks.BankSystem.AcountBuilding
{
    public abstract class AccountBuilder
    {
        private Bank _bank;
        public AccountBuilder(Bank bank, BankClient bankClient)
        {
            _bank = bank ?? throw new ArgumentNullException(nameof(bank));
            BankClient = bankClient ?? throw new ArgumentNullException(nameof(bankClient));
            DepositYearlyPercent = _bank.GetDepositPercentage(Balance);
            CreditCommission = _bank.CreditAccountCommission;
            DebitYearlyPercent = _bank.DebitAccountYearlyPercentage;
            TransactionLimit = _bank.TransactionLimit;
            AccountLimit = _bank.CreditAccountLimit;
            ExpirationDate = DateTime.Now.AddYears(2);
        }

        protected BankClient BankClient { get; }
        protected decimal Balance { get; set; }
        protected decimal CreditCommission { get; set; }
        protected decimal DepositYearlyPercent { get; set; }
        protected decimal DebitYearlyPercent { get; set; }
        protected decimal TransactionLimit { get; set; }
        protected decimal AccountLimit { get; set; }
        protected DateTime ExpirationDate { get; set; }
        public AccountBuilder SetInitialBalance(decimal balance)
        {
            Balance = balance;
            DepositYearlyPercent = _bank.GetDepositPercentage(balance);
            return this;
        }

        public AccountBuilder SetExpirationDate(DateTime date)
        {
            ExpirationDate = date;
            return this;
        }

        public BankAccount CreateAccountInTheBank()
        {
            BankAccount account = MakeAccount();
            _bank.AddAccount(account);
            return account;
        }

        public abstract BankAccount MakeAccount();
    }
}