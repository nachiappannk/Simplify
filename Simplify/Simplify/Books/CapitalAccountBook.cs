using System;
using System.Collections.Generic;
using System.Linq;

namespace Simplify.Books
{
    public class CapitalAccountBook : List<DatedStatement>
    {
        public static string OpeningCapital = "Opening Capital";
        public static string NetEarnings = "Net Earnings";
        public DateTime OpeningDate { get; set; }
        public DateTime ClosingDate { get; set; }

        public CapitalAccountBook(DateTime openingDate, DateTime closingDate)
        {
            OpeningDate = openingDate;
            ClosingDate = closingDate;
        }

    }

    public static class CapitalAccountBookExt
    {
        private static void InsertEntry(this CapitalAccountBook capitalAccount, string name,
            double value, DateTime date)
        {
           
                capitalAccount.Insert(0,new DatedStatement()
                {
                    Date = date,
                    Value = value,
                    Name = name,
                });
            
        }

        public static void InsertOpeningCapital(this CapitalAccountBook capitalAccount, double value)
        {
            capitalAccount.InsertEntry(CapitalAccountBook.OpeningCapital, value, capitalAccount.OpeningDate);
        }

        public static void InsertNetEarnings(this CapitalAccountBook capitalAccount, double value)
        {
            capitalAccount.InsertEntry(CapitalAccountBook.NetEarnings, value, capitalAccount.ClosingDate);
        }

        public static double GetCapital(this CapitalAccountBook capitalAccount)
        {
            return capitalAccount.GetTotal();
        }

    }
}