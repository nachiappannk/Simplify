using System;
using System.Collections.Generic;

namespace Simplify.Books
{
    public class DatedStatement : Statement
    {
        public DateTime Date { get; set; }
    }

    public static class DatedStatementExtention
    {
        public static DatedStatement CreateCopy(this DatedStatement datedStatement)
        {
            return new DatedStatement()
            {
                Date = datedStatement.Date,
                Description = datedStatement.Description,
                Value = datedStatement.Value,

            };
        }
    }
}