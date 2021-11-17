using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Banks.UpdatesSubscribers;
using Microsoft.EntityFrameworkCore;

namespace Banks.Client
{
    public class BankClient
    {
        private List<ClientSubscribedEvent> _subscribedEvents;
        public BankClient(string name, string surname)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            Address = null;
            PassportNumber = null;
            _subscribedEvents = new List<ClientSubscribedEvent>();
            Id = Guid.NewGuid();
        }

        public BankClient(BankClient other)
        {
            Name = other.Name;
            Surname = other.Surname;
            Address = other.Address;
            PassportNumber = other.PassportNumber;
            _subscribedEvents = other._subscribedEvents;
            Id = Guid.NewGuid();
        }

        public BankClient()
        {
            _subscribedEvents = new List<ClientSubscribedEvent>();
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Surname { get; init; }
        public string Address { get; private set; }
        public virtual PassportNumber PassportNumber { get; private set; }

        [BackingField(nameof(_subscribedEvents))]
        public virtual IReadOnlyCollection<ClientSubscribedEvent> SubscribedEvents => _subscribedEvents;

        [NotMapped]
        public bool TrustWorthy => Address != null && PassportNumber != null;
        public BankClient SetAddress(string address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            return this;
        }

        public BankClient SetPassportNumber(string number)
        {
            _ = number ?? throw new ArgumentNullException(nameof(number));
            PassportNumber = new PassportNumber(number);
            return this;
        }

        public BankClient Subscribe(EventType type)
        {
            if (!Subscribed(type))
            {
                _subscribedEvents.Add(new ClientSubscribedEvent(type));
            }

            return this;
        }

        public void Notify(BankTermsChange change)
        {
        }

        public bool Subscribed(EventType type)
        {
            return _subscribedEvents.Exists(x => x.Type == type);
        }

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != GetType()) return false;
            var other = (BankClient)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return PassportNumber.Number.GetHashCode();
        }
    }
}