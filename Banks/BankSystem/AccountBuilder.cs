using System;
using Banks.Accounts;
using Banks.Client;

namespace Banks.BankSystem
{
    public class AccountBuilder
    {
        private Bank _bank;
        private BankClient _bankClient;
        private AccountType _type;
        private decimal _balance;
        private decimal _creditCommission;
        private decimal _depositYearlyPercent;
        private decimal _debitYearlyPercent;
        private decimal _transactionLimit;
        private decimal _accountLimit;
        private DateTime _expirationDate;
        public AccountBuilder(Bank bank, BankClient bankClient, AccountType type)
        {
            _bank = bank ?? throw new ArgumentNullException(nameof(bank));
            _bankClient = bankClient ?? throw new ArgumentNullException(nameof(bankClient));
            _type = type;
            _depositYearlyPercent = _bank.GetDepositPercentage(_balance);
            _creditCommission = _bank.CreditAccountCommission;
            _debitYearlyPercent = _bank.DebitAccountYearlyPercentage;
            _transactionLimit = _bank.TransactionLimit;
            _accountLimit = _bank.CreditAccountLimit;
            _expirationDate = DateTime.Now.AddYears(2);
        }

        public AccountBuilder SetInitialBalance(decimal balance)
        {
            _balance = balance;
            _depositYearlyPercent = _bank.GetDepositPercentage(balance);
            return this;
        }

        public AccountBuilder SetExpirationDate(DateTime date)
        {
            _expirationDate = date;
            return this;
        }

        public BankAccount CreateAccount()
        {
            BankAccount account = MakeAccount();
            _bank.AddAccount(account);
            return account;
        }

        private BankAccount MakeAccount()
        {
            switch (_type)
            {
                case AccountType.Credit:
                    return new CreditAccount(_bankClient, _balance, _transactionLimit, _accountLimit, _creditCommission);
                case AccountType.Debit:
                    return new DebitAccount(_bankClient, _balance, _transactionLimit, _debitYearlyPercent);
                case AccountType.Deposit:
                    return new DepositAccount(_bankClient, _balance, _transactionLimit, _expirationDate, _depositYearlyPercent);
                default:
                    throw new ArgumentOutOfRangeException(nameof(_type));
            }
        }
    }
}