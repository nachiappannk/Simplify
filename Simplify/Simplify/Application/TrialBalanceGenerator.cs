using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Books;

namespace Simplify
{
    public class TrialBalanceGenerator
    {
        public TrialBalanceBook Generate(IList<JournalStatement> journalStatements)
        {
            var statements = journalStatements.GroupBy(
                    x => x.Name, 
                    y => y.Value, 
                    (key, values) => new Statement()
                    {
                        Name = key,
                        Value = values.Sum(),
                    })
                .ToList();
            var trialBalance = new TrialBalanceBook();

            trialBalance.AddRange(statements);
            return trialBalance;
        }
    }
}