using System.Linq;
using Banks.Accounts;
using Banks.BankSystem;
using Banks.Client;
using Banks.Transactions;
using Banks.UpdatesSubscribers;
using Microsoft.EntityFrameworkCore;

namespace Banks.ORM
{
    public sealed class CentralBankContext : DbContext
    {
        public CentralBankContext(DbContextOptions<CentralBankContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<CentralBank> CentralBanks { get; set; }
        public bool Contains(CentralBank bank)
        {
            return CentralBanks.Find(bank.Id) != null;
        }

        public CentralBank GetLast()
        {
            return CentralBanks.AsEnumerable().Last();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientSubscribedEvent>()
                .Property(x => x.Type)
                .HasConversion<int>();
            modelBuilder.Entity<PassportNumber>();
            modelBuilder.Entity<BankClient>();
            modelBuilder.Entity<PercentageGap>();
            modelBuilder.Entity<BankAccountTerms>();
            modelBuilder.Entity<BankAccount>();
            modelBuilder.Entity<CreditAccount>();
            modelBuilder.Entity<DebitAccount>();
            modelBuilder.Entity<DepositAccount>();
            modelBuilder.Entity<Transaction>();
            modelBuilder.Entity<CashTransaction>();
            modelBuilder.Entity<DepositTransaction>();
            modelBuilder.Entity<P2PTransaction>();

            modelBuilder.Entity<Bank>();
            modelBuilder.Entity<CentralBank>().ToTable("Central banks")
                .HasMany<BankClient>();
            base.OnModelCreating(modelBuilder);
        }
    }
}