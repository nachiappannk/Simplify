using System.Collections.Generic;
using System.Linq;
using Simplify.Books;

namespace Simplify.Application
{
    public class ProfitAndLossAccountGenerator
    {
        public ProfitAndLossBook Generate(IList<DetailedDatedStatement> journalStatements)
        {
            var neededStatements = journalStatements;
            var groupedStatements = neededStatements.GroupBy(x => x.Description, x => x.Value,
                (key, values) => new Statement()
                {
                    Description = key,
                    Value = values.Sum(),
                }).OrderBy(x=> x.Description).ToList();
            var profitAndLossBook = new ProfitAndLossBook();
            profitAndLossBook.AddRange(groupedStatements);
            return profitAndLossBook;
        }
    }
}