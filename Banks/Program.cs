﻿using System.Linq;
using Banks.Accounts;
using Banks.BankSystem;
using Banks.Client;
using Banks.ORM;
using Banks.Transactions;
using Banks.UI;
using Banks.UpdatesSubscribers;
using Microsoft.EntityFrameworkCore;

namespace Banks
{
    internal static class Program
    {
        private static void Main()
        {
            SaveStateToDb();
            var optionsBuilder = new DbContextOptionsBuilder<CentralBankContext>();
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlite(@"Data Source=CentralBankDB.db;");
            var db = new CentralBankContext(optionsBuilder.Options);
            CentralBank loadedBank = db.CentralBanks.ToList().Last();

            var ui = new UserInterface(loadedBank);
            ui.Run();
        }

        private static void SaveStateToDb()
        {
            CentralBank centralBank = ConstructTestCentralBank();
            var optionsBuilder = new DbContextOptionsBuilder<CentralBankContext>();
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlite(@"Data Source=CentralBankDB.db;");
            var db = new CentralBankContext(optionsBuilder.Options);
            db.CentralBanks.Add(centralBank);
            db.SaveChanges();
        }

        private static CentralBank ConstructTestCentralBank()
        {
            var centralBank = new CentralBank();
            var determinator = new BankAccountTerms(3, 10,  100, 100, -1000);
            determinator.AddGap(new PercentageGap(1, 4));
            determinator.AddGap(new PercentageGap(100,  5));
            determinator.AddGap(new PercentageGap(200, 6));
            var bank1 = new Bank("bank1", determinator);

            determinator.AddGap(new PercentageGap(300, 7));
            var bank2 = new Bank("bank2", determinator);
            centralBank.RegisterBank(bank1);
            centralBank.RegisterBank(bank2);
            var client1 = new BankClient("client", "1")
                .SetAddress("address1")
                .SetPassportNumber("12345678912");
            var client2 = new BankClient("client", "2")
                .SetAddress("address2")
                .SetPassportNumber("02345678912");
            centralBank.RegisterClient(client1);
            centralBank.RegisterClient(client2);
            var accountBuilder = new AccountBuilder(bank1, client1, AccountType.Credit)
                .SetInitialBalance(100);
            BankAccount account1 = accountBuilder.CreateAccount();
            accountBuilder = new AccountBuilder(bank2, client1, AccountType.Debit);
            accountBuilder.CreateAccount();
            accountBuilder = new AccountBuilder(bank1, client2, AccountType.Debit);
            BankAccount account2 = accountBuilder.CreateAccount();
            accountBuilder = new AccountBuilder(bank2, client2, AccountType.Deposit);
            accountBuilder.CreateAccount();
            Transaction transaction = new P2PTransaction(account1, account2, 200);
            centralBank.MakeTransaction(transaction);
            bank1.SubscribeClient(client1, EventType.CreditAccountTermsChange);
            bank2.SubscribeClient(client1, EventType.DebitAccountTermsChange);

            return centralBank;
        }
    }
}
