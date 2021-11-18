using System;
using Banks.Accounts;
using Banks.BankSystem;
using Banks.BankSystem.AcountBuilding;
using Banks.Client;
using Banks.Transactions;
using NUnit.Framework;

namespace Banks.Tests
{
    public class BanksTests
    {
        private CentralBank _centralBank;
        private Bank _testBank;
        private BankAccountTerms _testDeterminator;
        private BankClient _testClient;

        [SetUp]
        public void SetUp()
        {
            _centralBank = new CentralBank();
            
            _testDeterminator = new BankAccountTerms(3.65m, 3.65m, 100, 100, -1000);
            var gap1 = new PercentageGap(0, 0.365m);
            var gap2 = new PercentageGap(100,  3.65m);
            var gap3 = new PercentageGap(1000, 7.3m);
            
            _testDeterminator.AddGap(gap3);
            _testDeterminator.AddGap(gap2);
            _testDeterminator.AddGap(gap1);
            
            _testBank = new Bank("bank", _testDeterminator);
            
            _centralBank.RegisterBank(_testBank);

            _testClient = new BankClient("zu", "zua");
            _testClient.SetAddress("K.");
            _testClient.SetPassportNumber("8080228666");
        }
        [Test]
        public void TestClient()
        {
            var client = new BankClient("Ivan", "Ivanov");
            Assert.That(!client.TrustWorthy);
            client.SetAddress("K.");
            Assert.That(!client.TrustWorthy);
            client.SetPassportNumber("8080228666");
            Assert.That(client.TrustWorthy);
        }

        [Test]
        public void TestCashAndDepositDebitAccount()
        {
            decimal amount = 1100;
            var client = new BankClient("Ivan", "Ivanov");
            var debitAccount = new DebitAccount(client, 1000, 100, 10);
            debitAccount.Deposit(100);
            Assert.AreEqual(amount, debitAccount.Balance);
            Assert.Catch(() =>
            {
                debitAccount.Cash(200);
            });
            Assert.AreEqual(amount, debitAccount.Balance);
            client.SetAddress("K.");
            client.SetPassportNumber("8080228666");
            debitAccount.Cash(200);
            amount -= 200;
            Assert.AreEqual(amount, debitAccount.Balance);
        }

        [Test]
        public void TestCashAndDepositCreditAccount()
        {
            decimal amount = 1100;
            var client = new BankClient("Ivan", "Ivanov");
            var creditAccount = new CreditAccount(client, 1000, 100, -1000, 10);
            creditAccount.Deposit(100);
            Assert.AreEqual(creditAccount.Balance, amount);
            Assert.Catch(() =>
            {
                creditAccount.Cash(200);
            });
            Assert.AreEqual(amount, creditAccount.Balance);
            client.SetAddress("K.");
            client.SetPassportNumber("8080228666");
            creditAccount.Cash(1000);
            amount -= 1000;
            Assert.AreEqual(creditAccount.Balance, amount);
            
            Assert.Catch(() =>
            {
                creditAccount.Cash(2000);
            });
            
            Assert.AreEqual(creditAccount.Balance, amount);
        }
        [Test]
        public void TestCashDepositDepositAccount()
        {
            decimal amount = 1100;
            var client = new BankClient("Ivan", "Ivanov");
            var depositAccount = new DepositAccount(client, 1000, 1000, DateTime.Today.AddDays(1), 10);
            depositAccount.Deposit(100);
            Assert.AreEqual(depositAccount.Balance, amount);
            Assert.Catch(() =>
            {
                depositAccount.Cash(200);
            });
            Assert.AreEqual(amount, depositAccount.Balance);
            depositAccount = new DepositAccount(client, 1000, 1000, DateTime.MinValue, 10);
            
            depositAccount.Deposit(100);
            Assert.AreEqual(depositAccount.Balance, amount);
            depositAccount.Cash(200);
            amount -= 200;
            Assert.AreEqual(amount, depositAccount.Balance);
        }

        [Test]
        public void TestPercentageGapAndDeterminator()
        {
            var gap1 = new PercentageGap(1, 0);
            var gap2 = new PercentageGap(100,  2);
            var gap3 = new PercentageGap(200, 3);
            var gap4 = new PercentageGap(200, 4);
            var determinator = new BankAccountTerms(3, 10,  100, 100, -1000);
            determinator.AddGap(gap3);
            determinator.AddGap(gap2);
            determinator.AddGap(gap1);
            
            Assert.AreEqual(determinator.GetDepositPercentage(1), 0);
            Assert.AreEqual(determinator.GetDepositPercentage(100), 2);
            Assert.AreEqual(determinator.GetDepositPercentage(150), 2);
            Assert.AreEqual(determinator.GetDepositPercentage(200), 3);
            Assert.AreEqual(determinator.GetDepositPercentage(250), 3);
            Assert.AreEqual(determinator.GetDepositPercentage(0), 3);
            
            determinator.AddGap(gap4);
            Assert.AreEqual(determinator.GetDepositPercentage(200), 4);
            Assert.AreEqual(determinator.GetDepositPercentage(250), 4);
        }
        
        [Test]
        public void TestCompleteP2PTransactionsAndCancelTransactions()
        {
            decimal balance1 = 10000;
            decimal balance2 = 10000;
            var client1 = new BankClient("beb", "ra");
            var client2 = new BankClient("shr", "eck");
            var account1 = new DebitAccount(client1, 10000, 1000, 3);
            var account2 = new DebitAccount(client2, 10000, 1000, 3);

            var p2pTransaction = new P2PTransaction(account1, account2, 100);
            balance1 -= 100;
            balance2 += 100;
            p2pTransaction.Complete();
            Assert.AreEqual(account1.Balance, balance1);
            Assert.AreEqual(account2.Balance, balance2);
            balance1 += 100;
            balance2 -= 100;
            p2pTransaction.Reverse();
            Assert.AreEqual(account1.Balance, balance1);
            Assert.AreEqual(account2.Balance, balance2);
        }
        
        [Test]
        public void TestCompleteCashTransactionsAndCancelTransactions()
        {
            decimal balance1 = 10000;
            var client1 = new BankClient("beb", "ra");
            var account1 = new DebitAccount(client1, 10000, 1000, 3);


            var cashTransaction = new CashTransaction(account1, 100);
            balance1 -= 100;
            cashTransaction.Complete();
            Assert.AreEqual(account1.Balance, balance1);
            balance1 += 100;
            cashTransaction.Reverse();
            Assert.AreEqual(account1.Balance, balance1);
        }
        
        [Test]
        public void TestCompleteDepositTransactionsAndCancelTransactions()
        {
            decimal balance1 = 10000;
            var client1 = new BankClient("beb", "ra");
            var account1 = new DebitAccount(client1, 10000, 1000, 3);


            var depositTransaction = new DepositTransaction(account1, 100);
            balance1 += 100;
            depositTransaction.Complete();
            Assert.AreEqual(account1.Balance, balance1);
            balance1 -= 100;
            depositTransaction.Reverse();
            Assert.AreEqual(account1.Balance, balance1);
        }

        [Test]
        public void TestDepositCommissions()
        {
            decimal initialBalance = 100;
            var builder = new DepositAccountBuilder(_testBank, _testClient);
            builder.SetInitialBalance(initialBalance);
            BankAccount account = builder.CreateAccountInTheBank();

            decimal yearPercentage = _testDeterminator.GetDepositPercentage(initialBalance);
            decimal expectedBalance = initialBalance;
            DateTime now = DateTime.Now;
            for (DateTime current = now; current < now.AddYears(1); current = current.AddMonths(1))
            {
                for (DateTime currentDays = current; currentDays < current.AddMonths(1); currentDays = currentDays.AddDays(1))
                {
                    expectedBalance *= (1 + yearPercentage / 36500m);
                }
            }
            BankAccount expectedAccountState = account.GetAccountStateByDate(now.AddYears(1));
            Assert.That(Math.Abs(expectedBalance - expectedAccountState.Balance) < 0.01m);
        }

        [Test]
        public void TestDebitCommissions()
        {
            decimal initialBalance = 100;
            var builder = new DebitAccountBuilder(_testBank, _testClient);
            builder.SetInitialBalance(initialBalance);
            BankAccount account = builder.CreateAccountInTheBank();

            decimal dailyPercentage = _testDeterminator.DebitAccountYearlyPercentage / 365m;
            decimal expectedBalance = initialBalance;
            DateTime now = DateTime.Now;
            for (DateTime current = now; current < now.AddYears(1); current = current.AddMonths(1))
            {
                for (DateTime currentDays = current; currentDays < current.AddMonths(1); currentDays = currentDays.AddDays(1))
                {
                    expectedBalance *= (1 + dailyPercentage / 100m);
                }
            }

            BankAccount expectedAccountState = account.GetAccountStateByDate(now.AddYears(1));
            Assert.That(Math.Abs(expectedBalance - expectedAccountState.Balance) < 0.01m);
        }

        [Test]
        public void TestCreditCommission()
        {
            decimal initialBalance = -100;
            var builder = new CreditAccountBuilder(_testBank, _testClient);
            builder.SetInitialBalance(initialBalance);
            BankAccount account = builder.CreateAccountInTheBank();

            decimal commission = _testDeterminator.CreditAccountCommission;
            account.Cash(100);
            _centralBank.PayCommissions();
            decimal expectedBalance = initialBalance - 100 - _testDeterminator.CreditAccountCommission;
            Assert.AreEqual(account.Balance, expectedBalance);
            
            account.Deposit(300);
            _centralBank.PayCommissions();
            expectedBalance += 300;
            Assert.AreEqual(account.Balance, expectedBalance);
        }
    }
}