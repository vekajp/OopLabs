using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Banks.Accounts;
using Banks.Client;
using Banks.UpdatesSubscribers;
using Microsoft.EntityFrameworkCore;

namespace Banks.BankSystem
{
    public class Bank
    {
        private List<BankClient> _clients;
        private List<BankAccount> _bankAccounts;
        public Bank(string name, BankAccountTerms accountTerms)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AccountTerms = accountTerms;
            _clients = new List<BankClient>();
            _bankAccounts = new List<BankAccount>();
            Id = Guid.NewGuid();
        }

        public Bank()
        {
            _clients = new List<BankClient>();
            _bankAccounts = new List<BankAccount>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public virtual BankAccountTerms AccountTerms { get; private set; }

        [BackingField(nameof(_bankAccounts))]
        public virtual IReadOnlyCollection<BankAccount> BankAccounts => _bankAccounts;
        [BackingField(nameof(_clients))]
        public virtual IReadOnlyCollection<BankClient> Clients => _clients;
        public string Name { get; init; }
        [NotMapped]
        public decimal TransactionLimit
        {
            get => AccountTerms.TransactionLimit;
        }

        [NotMapped]
        public decimal CreditAccountLimit
        {
            get => AccountTerms.CreditAccountLimit;
        }

        [NotMapped]
        public decimal DebitAccountYearlyPercentage
        {
            get => AccountTerms.DebitAccountYearlyPercentage;
        }

        [NotMapped]
        public decimal CreditAccountCommission
        {
            get => AccountTerms.CreditAccountCommission;
        }

        public void AddAccount(BankAccount account)
        {
            if (BankAccounts.Contains(account))
            {
                throw new ArgumentException("Account was already registered", nameof(account));
            }

            _bankAccounts.Add(account);
            if (!Registered(account.Client))
            {
                AddClient(account.Client);
            }
        }

        public void AddClient(BankClient client)
        {
            if (Clients.Contains(client))
            {
                throw new ArgumentException("Client was already registered", nameof(client));
            }

            _clients.Add(new BankClient(client));
        }

        public void ChangeTransactionLimit(decimal newLimit)
        {
            AccountTerms.ChangeTransactionLimit(newLimit);
            string message = $"Transaction limit has changed. New limit is {newLimit}";
            var changeCredit = new CreditAccountTermsChange(message);
            var changeDebit = new DebitAccountTermsChange(message);
            var changeDeposit = new DepositAccountTermsChange(message);
            NotifySubscribers(changeCredit, EventType.CreditAccountTermsChange);
            NotifySubscribers(changeDebit, EventType.DebitAccountTermsChange);
            NotifySubscribers(changeDeposit, EventType.DepositAccountTermsChange);
        }

        public void ChangeCreditAccountLimit(decimal newLimit)
        {
            AccountTerms.ChangeCreditAccountLimit(newLimit);
            var change = new CreditAccountTermsChange($"Credit account limit has changed. New limit is {newLimit}.");
            NotifySubscribers(change, EventType.CreditAccountTermsChange);
        }

        public void ChangeDebitAccountYearlyPercentage(decimal value)
        {
            AccountTerms.ChangeDebitAccountYearlyPercentage(value);
            var change = new DebitAccountTermsChange($"Debit account yearly percentage has changed. New percentage is {value}.");
            NotifySubscribers(change, EventType.DebitAccountTermsChange);
        }

        public void ChangeCreditAccountCommission(decimal value)
        {
            AccountTerms.ChangeCreditAccountCommission(value);
            var change = new CreditAccountTermsChange($"Credit account commission has changed. New commission is {value}.");
            NotifySubscribers(change, EventType.CreditAccountTermsChange);
        }

        public void RecountAccounts()
        {
            _bankAccounts.ForEach(x => x.RecountCommission());
        }

        public void PayCommission()
        {
            _bankAccounts.ForEach(x => x.PayCommission());
        }

        public decimal GetDepositPercentage(decimal initialBalance)
        {
            return AccountTerms.GetDepositPercentage(initialBalance);
        }

        public override string ToString()
        {
            return Name;
        }

        public void SubscribeClient(BankClient client, EventType type)
        {
            if (!_bankAccounts.Exists(x => x.Client == client))
            {
                throw new ArgumentException("Cannot subscribe this client");
            }

            client.Subscribe(type);
        }

        private void NotifySubscribers(BankTermsChange change, EventType type)
        {
            foreach (BankClient client in _clients.Where(client => client.Subscribed(type)))
            {
                client.Notify(change);
            }
        }

        private bool Registered(BankClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (Clients is null)
            {
                throw new ArgumentNullException(nameof(Clients));
            }

            return Clients.Contains(client);
        }
    }
}