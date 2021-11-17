using System.Collections.Generic;
using System.Linq;
using Banks.BankSystem;
using Banks.Client;
using Banks.Transactions;
using Spectre.Console;

namespace Banks.UI.ClientActions
{
    public class CancelTransaction : ClientActionCommand
    {
        public CancelTransaction(CentralBank centralBank, BankClient client)
            : base(centralBank, client, "Cancel transaction")
        {
        }

        public override void Execute()
        {
            Transaction transaction = GetTransaction();
            if (transaction == null) return;
            try
            {
                CentralBank.ReverseTransaction(transaction);
            }
            catch
            {
                AnsiConsole.WriteLine("Unable to cancel transaction");
            }
        }

        private Transaction GetTransaction()
        {
            IEnumerable<Transaction> transactions =
                CentralBank.Transactions;
            int transactionsCount = transactions.Count();
            Transaction transaction = AnsiConsole.Prompt(new SelectionPrompt<Transaction>()
                .Title("Select transaction")
                .PageSize(transactionsCount > 3 ? transactionsCount : 3)
                .AddChoices(transactions));
            return transaction;
        }
    }
}