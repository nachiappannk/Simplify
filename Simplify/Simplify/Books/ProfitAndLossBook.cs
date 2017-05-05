using System.Collections.Generic;

namespace Simplify.Books
{
    public class ProfitAndLossBook : List<Statement>
    {
    }

    public static class ProfitAndLossExt
    {
        public static double GetNetEarnings(this ProfitAndLossBook profitAndLossBook)
        {
            return profitAndLossBook.GetTotal();
        }
    }
}