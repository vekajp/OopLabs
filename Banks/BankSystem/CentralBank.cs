using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Accounts;
using Banks.Client;
using Banks.Tools;
using Banks.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Banks.BankSystem
{
    public class CentralBank
    {
        private List<Bank> _banks;
        private List<BankClient> _clients;
        private List<Transaction> _transactions;
        public CentralBank()
        {
            _banks = new List<Bank>();
            _clients = new List<BankClient>();
            _transactions = new List<Transaction>();
            Id = Guid.NewGuid();
        }

        public CentralBank(CentralBank other)
        {
            _banks = other._banks;
            _transactions = other._transactions;
            _clients = other._clients;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        [BackingField(nameof(_banks))]
        public virtual IReadOnlyCollection<Bank> Banks => _banks;

        [BackingField(nameof(_clients))]
        public virtual IReadOnlyCollection<BankClient> Clients => _clients;

        [BackingField(nameof(_transactions))]
        public virtual IReadOnlyCollection<Transaction> Transactions => _transactions;
        public void RegisterBank(Bank bank)
        {
            if (_banks.Contains(bank))
            {
                throw new ArgumentException("Bank was already registered", nameof(bank));
            }

            _banks.Add(bank);
        }

        public void Initialize(CentralBank other)
        {
            _banks = other._banks;
            _transactions = other._transactions;
            _clients = other._clients;
            Id = other.Id;
        }

        public void OpenNewAccount(Bank bank, BankAccount account)
        {
            bank.AddAccount(account);
        }

        public void OnEveryDayUpdate()
        {
            _banks.ForEach(x => x.RecountAccounts());
        }

        public void PayCommissions()
        {
            _banks.ForEach(x => x.PayCommission());
        }

        public void RegisterClient(BankClient client)
        {
            if (!_clients.Contains(client))
            {
                _clients.Add(client);
            }
        }

        public void MakeTransaction(Transaction transaction)
        {
            if (!transaction.Complete())
            {
                throw new TransactionCannotBeCompletedException();
            }

            _transactions.Add(transaction);
        }

        public void ReverseTransaction(Transaction transaction)
        {
            if (!Transactions.Contains(transaction))
            {
                throw new TransactionCannotBeReversedException("Transaction doesn't exist");
            }

            transaction.Reverse();
            _transactions.Remove(transaction);
        }
    }
}