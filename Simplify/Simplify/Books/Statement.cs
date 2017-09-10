using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace Simplify.Books
{
    public class Statement
    {
        public string Description { get; set; }
        public double Value { get; set; }
    }


    public static class StatementsExtentions
    {

        private static bool IsZero(double value)
        {
            return Math.Abs(value) < 0.001;
        }

        public static bool IsBalanced(this IEnumerable<Statement> statements)
        {
            var total = statements.GetTotal();
            return IsZero(total);
        }

        public static double GetTotal(this IEnumerable<Statement> statements)
        {
            var total = statements.Sum(s => s.Value);
            if (IsZero(total)) return 0;
            return total;
        }

        public static double GetCreditTotal(this IEnumerable<Statement> statements)
        {
            return statements.Where(x => x.Value > 0).GetTotal();
        }

        public static double GetDebitTotal(this IEnumerable<Statement> statements)
        {
            return -1 * statements.Where(x => x.Value <= 0).GetTotal();
        }

        public static double GetCreditValue(this Statement statement)
        {
            return statement.Value > 0 ? statement.Value : 0;
        }

        public static double GetDebitValue(this Statement statement)
        {
            return statement.Value <= 0 ? -1 * statement.Value : 0;
        }
    }
}