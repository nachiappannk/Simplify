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

        public List<Statement> GetSummaryStatement()
        {
            return this.GroupBy(x => x.Description, 
                y => y.Value, 
                (key, results) => new Statement() { Description = key, Value = results.Sum() }).ToList();

        }
    }
}