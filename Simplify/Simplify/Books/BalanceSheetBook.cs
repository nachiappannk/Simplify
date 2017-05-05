using System.Collections.Generic;
using System.Linq;

namespace Simplify.Books
{
    public class BalanceSheetBook : List<Statement>
    {
        public static string Capital = "Capital";    
    }

    public static class BalanceSheetExtentions
    {
        public static double GetCapital(this BalanceSheetBook balanceSheet)
        {
            var capitalStatement = balanceSheet.FirstOrDefault(x => x.Name == BalanceSheetBook.Capital);
            if (capitalStatement != null)
            {
                return capitalStatement.Value;
            }
            return 0;
        }

        public static void UpsertCapital(this BalanceSheetBook balanceSheet, double capital)
        {
            var capitalStatement = balanceSheet.FirstOrDefault(x => x.Name == BalanceSheetBook.Capital);
            if (capitalStatement != null)
            {
                capitalStatement.Value = capital;
            }
            else
            {
                balanceSheet.Add(new Statement()
                    {
                        Name = BalanceSheetBook.Capital,
                        Value = capital,
                    }
                );
            }
        }
    }
}