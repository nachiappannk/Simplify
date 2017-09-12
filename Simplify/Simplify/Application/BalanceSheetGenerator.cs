using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Simplify.Books;

namespace Simplify.Application
{
    public class BalanceSheetGenerator
    {
        public BalanceSheetBook Generate(IList<DetailedDatedStatement> statements)
        {
            var balanceSheetBook = new BalanceSheetBook();
            var balanceSheetStatements = statements
                .GroupBy(x => x.Description, y => y.Value, (description, values) => new Statement()
                {
                    Description = description,
                    Value = values.Sum(),
                });

            balanceSheetBook.AddRange(balanceSheetStatements);
            return balanceSheetBook;
        }



        public BalanceSheetBook Generate(IList<DetailedDatedStatement> journalStatements,
            BalanceSheetBook previousYearBalanceSheet, double capital)
        {

            var currentYearStatements = journalStatements
                //.Select(x => (Statement) x)
                .Select(TrimBrackets)
                .Where(s => Math.Abs(s.Value) > 0.001).ToList();


            var allStatements = new List<Statement>();
            allStatements.AddRange(previousYearBalanceSheet);
            allStatements.AddRange(currentYearStatements);


            var groupedStatements = allStatements
                .GroupBy(x => x.Description, x => x.Value, (key, values) =>
                    new Statement()
                    {
                        Description = key,
                        Value = values.Sum(),
                    })
                .Where(s => Math.Abs(s.Value) > 0.001).ToList();

            

            var balanceSheet = new BalanceSheetBook();
            balanceSheet.AddRange(groupedStatements.OrderBy(s => s.Description));
            balanceSheet.UpsertCapital(capital);
            return balanceSheet;
        }

        private Statement TrimBrackets(Statement s)
        {
            var name = s.Description;
            var output = Regex.Replace(name, "\\([a-zA-Z0-9\\s]*\\)", string.Empty);
            return new Statement()
            {
                Description = output.Trim(),
                Value = s.Value,
            };
        }
    }
}