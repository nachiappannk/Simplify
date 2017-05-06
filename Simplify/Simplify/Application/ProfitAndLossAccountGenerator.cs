using System.Collections.Generic;
using System.Linq;
using Simplify.Books;

namespace Simplify.Application
{
    public class ProfitAndLossAccountGenerator
    {
        public ProfitAndLossBook Generate(IList<JournalStatement> journalStatements)
        {
            var neededStatements = journalStatements.Where(x => x.BookName == Book.ProfitAndLoss);
            var groupedStatements = neededStatements.GroupBy(x => x.Name, x => x.Value,
                (key, values) => new Statement()
                {
                    Name = key,
                    Value = values.Sum(),
                }).ToList();
            var profitAndLossBook = new ProfitAndLossBook();
            profitAndLossBook.AddRange(groupedStatements);
            return profitAndLossBook;
        }
    }
}