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
            
            var neededStatements = journalStatements.ToList();
            var capitalAccount = new CapitalAccountBook(accountOpeningDate, accountClosingDate);
            capitalAccount.AddRange(neededStatements.OrderBy(x=> x.Date));
            capitalAccount.InsertNetEarnings(netEarnings);
            capitalAccount.InsertOpeningCapital(openingBalance);
            return capitalAccount;
        }
    }
}