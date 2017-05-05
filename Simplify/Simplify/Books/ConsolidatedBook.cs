using System.Collections.Generic;

namespace Simplify.Books
{
    public class ConsolidatedBook
    {
        public IList<JournalStatement> Journal { get; set; }
        public TrialBalanceBook TrialBalance { get; set; }
        public ProfitAndLossBook ProfitAndLoss { get; set; }
        public CapitalAccountBook CapitalAccount { get; set; }
        public BalanceSheetBook BalanceSheetBook { get; set; }
        public BalanceSheetBook PreviousBalanceSheet { get; set; }

    }
}