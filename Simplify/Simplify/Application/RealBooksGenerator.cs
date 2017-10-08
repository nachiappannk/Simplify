using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Books;

namespace Simplify.Application
{
    public class RealBooksGenerator
    {
        private readonly StatementPriorityAdjuster _statementPriorityAdjuster = new StatementPriorityAdjuster();

        Dictionary<string,List<DatedStatement>> _accounts = new Dictionary<string, List<DatedStatement>>();

        public List<RealAccountBook> GetRealAccountBooks(List<string> capitalAccountNames)
        {
            var nonInvertedRealAccountBooks = GetRealAccountBooksWithOutInversion();
            return nonInvertedRealAccountBooks.Select(x =>
            {
                var realAccountBook = new RealAccountBook(x.AccountName);
                if (capitalAccountNames.Contains(x.AccountName))
                {
                    realAccountBook.AddRange(x);
                }
                else
                {
                    var datedStatements = x.Select(z =>
                        new DatedStatement() { Date = z.Date, Description = z.Description, Value = -1 * z.Value }).ToList();
                    realAccountBook.AddRange(datedStatements);
                }
                return realAccountBook;
            }).ToList();
        }

        private List<RealAccountBook> GetRealAccountBooksWithOutInversion()
        {
            return _accounts.Select(x =>
            {
                var realAccountBook = new RealAccountBook(x.Key);
                var datedStatements = x.Value.ToList();
                datedStatements = datedStatements.OrderBy(x1 => x1.Date).ToList();
                realAccountBook.AddRange(datedStatements);
                return realAccountBook;
            }).ToList();
        }

        public BalanceSheetBook GetBalanceSheetBook(List<string> captialAccountNames)
        {
            var realBooks = GetRealAccountBooksWithOutInversion();
            var balanceSheetStatements = realBooks.Select(x => new Statement() {Description = x.AccountName, Value = x.Sum(y => y.Value)}).ToList();
            foreach (var balanceSheetStatement in balanceSheetStatements)
            {
                if (captialAccountNames.Contains(balanceSheetStatement.Description))
                    balanceSheetStatement.Description = balanceSheetStatement.Description + " (E)";
            }
            
            balanceSheetStatements = balanceSheetStatements.OrderByDescending(x =>
            {
                var format = "00000000000";
                if (captialAccountNames.Contains(x.Description)) return "A"+x.Value.ToString(format);
                else if (x.Value > 0) return "C" + x.Value.ToString(format);
                else return "B" + x.Value.ToString(format);
            }).ToList();


            var balanceSheet = new BalanceSheetBook();
            balanceSheet.AddRange(balanceSheetStatements);
            return balanceSheet;
        }


        public void AddStatements(IEnumerable<DetailedDatedStatement> statements, StatementPriority priority)
        {
             foreach(var statement in statements)
                AddStatement(statement, priority);   
        }

        public void AddStatement(DetailedDatedStatement statement, StatementPriority priority)
        {
            var accountName = statement.Description;
            if(!_accounts.ContainsKey(accountName))
                _accounts.Add(accountName, new List<DatedStatement>());

            var datedStatement = new DatedStatement()
            {
                Date = statement.Date,
                Description = statement.DetailedDescription,
                Value = statement.Value,
            };
            _statementPriorityAdjuster.RePrioritizeStatement(statement, priority);
            _accounts[accountName].Add(datedStatement);
        }
    }
}