using System.Collections.Generic;

namespace Simplify.Books
{
    public class ConsolidatedBook
    {
        public IList<DetailedDatedStatement> Journal { get; set; }
        public TrialBalanceBook TrialBalance { get; set; }
        public List<NotionalAccountBook> NotionalAccountBooks { get; set; }

        public List<RealAccountBook> RealAccountBooks { get; set; }
        public BalanceSheetBook BalanceSheetBook { get; set; }

    }
}