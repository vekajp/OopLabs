using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Accounts;
using Banks.BankSystem;
using Banks.Client;
using Banks.Transactions;
using Spectre.Console;

namespace Banks.UI.ClientActions
{
    public class MakeTransaction : ClientActionCommand
    {
        private const int OptionsCount = 3;
        private BankAccount _initiator;
        public MakeTransaction(CentralBank centralBank, BankClient client)
            : base(centralBank, client, "Make transaction")
        {
        }

        public override void Execute()
        {
            _initiator = ChooseAccount(account => Equals(account.Client, Client), "Select your account");
            if (_initiator == null) return;
            TransactionType type = GetTransactionType();
            Transaction transaction = CreateTransaction(type);
            try
            {
                transaction.Complete();
            }
            catch (Exception e)
            {
                AnsiConsole.Markup($"[red bold]Haram![/] {e.Message}");
            }
        }

        private BankAccount ChooseAccount(Predicate<BankAccount> predicate, string selectorTitle)
        {
            var accounts = new List<BankAccount>();
            foreach (Bank bank in CentralBank.Banks)
            {
                accounts.AddRange(bank.BankAccounts.Where(account => predicate(account)));
            }

            if (accounts.Count == 0) return null;
            int accountsCount = accounts.Count;
            BankAccount account = AnsiConsole.Prompt(
                new SelectionPrompt<BankAccount>()
                    .Title(selectorTitle)
                    .PageSize(accountsCount > 3 ? accountsCount : 3)
                    .AddChoices(accounts));
            return account;
        }

        private TransactionType GetTransactionType()
        {
            TransactionType type = AnsiConsole.Prompt(
                new SelectionPrompt<TransactionType>()
                    .Title("Select transaction type")
                    .PageSize(OptionsCount)
                    .AddChoices(new TransactionType[]
                    {
                        TransactionType.P2PTransaction,
                        TransactionType.Cash,
                        TransactionType.Deposit,
                    }));
            return type;
        }

        private Transaction CreateTransaction(TransactionType type)
        {
            decimal amount = GetAmount();
            switch (type)
            {
                case TransactionType.P2PTransaction:
                    BankAccount receiver = ChooseAccount(account => !Equals(account, _initiator), "Select receiver account");
                    return new P2PTransaction(_initiator, receiver, amount);
                case TransactionType.Cash:
                    return new WithdrawalTransaction(_initiator, amount);
                case TransactionType.Deposit:
                    return new DepositTransaction(_initiator, amount);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private decimal GetAmount()
        {
            return AnsiConsole.Ask<decimal>("Enter amount");
        }
    }
}