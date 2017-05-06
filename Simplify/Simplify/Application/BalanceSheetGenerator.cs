using System.Collections.Generic;
using System.Linq;
using Simplify.Books;

namespace Simplify.Application
{
    public class BalanceSheetGenerator
    {
        public BalanceSheetBook Generate(IList<JournalStatement> journalStatements,
            BalanceSheetBook previousYearBalanceSheet, double capital)
        {
            var neededStatements = journalStatements.Where(x => x.BookName == Book.BalanceSheet)
                .Select(x => (Statement) x).ToList();
            neededStatements.AddRange(previousYearBalanceSheet);

            var groupedStatements = neededStatements.GroupBy(x => x.Name, x => x.Value, (key, values) =>
                new Statement()
                {
                    Name = key,
                    Value = values.Sum(),
                }).ToList();

            var balanceSheet = new BalanceSheetBook();
            balanceSheet.AddRange(groupedStatements);
            balanceSheet.UpsertCapital(capital);
            return balanceSheet;
        }
    }
}