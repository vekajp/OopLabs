using System;
using System.Globalization;
using Banks.Accounts;
using Banks.BankSystem;
using Banks.BankSystem.AcountBuilding;
using Banks.Client;
using Banks.UpdatesSubscribers;
using Spectre.Console;

namespace Banks.UI.ClientActions
{
    public class OpenAccount : ClientActionCommand
    {
        private const int OptionsCount = 3;
        public OpenAccount(CentralBank centralBank, BankClient client)
            : base(centralBank, client, "Open account")
        {
        }

        public override void Execute()
        {
            Bank bank = SelectBank();
            AccountType type = SelectAccountType();
            AccountBuilder accountBuilder = GetAccountBuilder(bank, Client, type);
            GetAccountExtraData(accountBuilder, type);
            accountBuilder.CreateAccountInTheBank();

            if (AnsiConsole.Confirm("Get account notifications?"))
            {
                bank.SubscribeClient(Client, GetEventType(type));
            }
        }

        private AccountBuilder GetAccountBuilder(Bank bank, BankClient client, AccountType type)
        {
            return type switch
            {
                AccountType.Credit => new CreditAccountBuilder(bank, client),
                AccountType.Debit => new DebitAccountBuilder(bank, client),
                AccountType.Deposit => new DepositAccountBuilder(bank, client),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private Bank SelectBank()
        {
            int bankCount = CentralBank.Banks.Count;
            Bank bank = AnsiConsole.Prompt(
                new SelectionPrompt<Bank>()
                    .Title("Select bank")
                    .PageSize(bankCount > 3 ? bankCount : 3)
                    .AddChoices(CentralBank.Banks));
            return bank;
        }

        private AccountType SelectAccountType()
        {
            AccountType type = AnsiConsole.Prompt(
                new SelectionPrompt<AccountType>()
                    .Title("Select bank")
                    .PageSize(OptionsCount)
                    .AddChoices(new[]
                    {
                        AccountType.Credit,
                        AccountType.Debit,
                        AccountType.Deposit,
                    }));
            return type;
        }

        private EventType GetEventType(AccountType type)
        {
            return type switch
            {
                AccountType.Credit => EventType.CreditAccountTermsChange,
                AccountType.Debit => EventType.DebitAccountTermsChange,
                AccountType.Deposit => EventType.DepositAccountTermsChange,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private void GetAccountExtraData(AccountBuilder builder, AccountType type)
        {
            builder.SetInitialBalance(GetInitialBalance());
            if (type == AccountType.Deposit && AnsiConsole.Confirm("Set custom expiration date?"))
            {
                builder.SetExpirationDate(GetExpirationDate());
            }
        }

        private DateTime GetExpirationDate()
        {
            const string format = "dd.MM.yyyy";
            DateTime expirationDate = DateTime.MinValue;
            while (expirationDate == DateTime.MinValue)
            {
                try
                {
                    string date = AnsiConsole.Ask<string>("Expiration date(dd.mm.yyyy)");
                    expirationDate = DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
                }
                catch
                {
                    AnsiConsole.Markup("[bold red] Haram![/] Wrong data format\n");
                }
            }

            return expirationDate;
        }

        private decimal GetInitialBalance()
        {
            return AnsiConsole.Ask<decimal>("Initial balance");
        }
    }
}