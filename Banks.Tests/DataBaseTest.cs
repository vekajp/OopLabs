using System;
using System.Linq;
using Banks.Accounts;
using Banks.BankSystem;
using Banks.BankSystem.AcountBuilding;
using Banks.Client;
using Banks.ORM;
using Banks.Transactions;
using Banks.UpdatesSubscribers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Banks.Tests
{
    public class DataBaseTest
    {
        [Test]
        public void AddSystemToDataBaseAndReceiveIt_CheckStatus()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CentralBankContext>();
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseInMemoryDatabase("CentralBankDB.db;");
            using var db = new CentralBankContext(optionsBuilder.Options);

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
            var accountBuilder = new CreditAccountBuilder(bank1, client1)
                .SetInitialBalance(100);
            BankAccount account1 = accountBuilder.CreateAccountInTheBank();
            accountBuilder = new DebitAccountBuilder(bank2, client1);
            accountBuilder.CreateAccountInTheBank();
            accountBuilder = new DebitAccountBuilder(bank1, client2);
            BankAccount account2 = accountBuilder.CreateAccountInTheBank();
            accountBuilder = new DepositAccountBuilder(bank2, client2);
            accountBuilder.CreateAccountInTheBank();
            Transaction transaction = new P2PTransaction(account1, account2, 200);
            centralBank.MakeTransaction(transaction);
            bank1.SubscribeClient(client1, EventType.CreditAccountTermsChange);
            bank2.SubscribeClient(client1, EventType.DebitAccountTermsChange);
            BankClient client3 = new BankClient("veka", "jp")
                .SetAddress("a")
                .SetPassportNumber("2286661337");

            accountBuilder = new DebitAccountBuilder(bank1, client3);
            accountBuilder.CreateAccountInTheBank();
            centralBank.RegisterClient(client3);
            Console.WriteLine(bank1.Clients.Count);
            db.CentralBanks.Add(centralBank);
            db.SaveChanges();
            
            var cbs = db.CentralBanks.ToList();
            Assert.AreEqual(1, cbs.Count);
            CentralBank cb = cbs.Last();
            Assert.AreEqual(2, cb.Banks.Count);
            Assert.AreEqual(3, cb.Clients.Count);
        }
    }
}