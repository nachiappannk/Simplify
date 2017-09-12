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

        public List<RealAccountBook> GetRealAccountBooks()
        {
            return _accounts.Select(x =>
            {
                var realAccountBook = new RealAccountBook(x.Key);
                var datedStatements = x.Value.Select(y => new DatedStatement() {Date =y.Date, Description = y.Description, Value = y.Value * -1});
                realAccountBook.AddRange(datedStatements);
                return realAccountBook;
            }).ToList();
        }

        public BalanceSheetBook GetBalanceSheetBook()
        {
            var realBooks = GetRealAccountBooks();
            var balanceSheetStatements = realBooks.Select(x => new Statement() {Description = x.AccountName, Value = x.Sum(y => y.Value)});
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