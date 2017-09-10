using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Application;

namespace Simplify.Books
{
    public class NotionalAccountBook : List<DetailedDatedStatement>
    {
        public NotionalAccountBook(NotionalAccount account)
        {
            Account = account;
        }
        public NotionalAccount Account { get; set; }

        public DetailedDatedStatement GetClosingStatment(DateTime closingDate)
        {
            var detailedStatement = new DetailedDatedStatement();
            detailedStatement.Date = closingDate;
            detailedStatement.Value = this.Sum(x => x.Value);
            detailedStatement.Description = Account.RealAccountName;
            detailedStatement.DetailedDescription = "Closing of "+Account.NotionalAccountName;


            return detailedStatement;
        }

        public List<Statement> GetSummaryStatement()
        {
            return this.GroupBy(x => x.Description, 
                y => y.Value, 
                (key, results) => new Statement() { Description = key, Value = results.Sum() }).ToList();

        }
    }
}