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
        private static void UpsertEntry(this CapitalAccountBook capitalAccount, string name,
            double value, DateTime date)
        {
            var statement = capitalAccount.FirstOrDefault(s => s.Name == name);
            if (statement != null)
            {
                statement.Date = date;
                statement.Value = value;
                statement.Name = name;
            }
            else
            {
                capitalAccount.Add(new DatedStatement()
                {
                    Date = date,
                    Value = value,
                    Name = name,
            });
            }
        }

        public static void UpsertOpeningCapital(this CapitalAccountBook capitalAccount, double value)
        {
            capitalAccount.UpsertEntry(CapitalAccountBook.OpeningCapital, value, capitalAccount.OpeningDate);
        }

        public static void UpsertNetEarnings(this CapitalAccountBook capitalAccount, double value)
        {
            capitalAccount.UpsertEntry(CapitalAccountBook.NetEarnings, value, capitalAccount.ClosingDate);
        }

        public static double GetCapital(this CapitalAccountBook capitalAccount)
        {
            return capitalAccount.GetTotal();
        }

    }
}