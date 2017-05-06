using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Books;

namespace Simplify.Application
{
    public class CapitalAccountGenerator
    {
        public CapitalAccountBook Generate(IList<JournalStatement> journalStatements, double openingBalance,
            double netEarnings, DateTime accountOpeningDate, DateTime accountClosingDate)
        {
            
            var neededStatements = journalStatements.Where(x => x.BookName == Book.Capital).ToList();
            var capitalAccount = new CapitalAccountBook(accountOpeningDate, accountClosingDate);
            capitalAccount.AddRange(neededStatements);
            capitalAccount.UpsertOpeningCapital(openingBalance);
            capitalAccount.UpsertNetEarnings(netEarnings);
            return capitalAccount;
        }
    }
}