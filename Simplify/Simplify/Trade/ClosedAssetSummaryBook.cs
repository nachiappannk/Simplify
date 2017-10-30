using System.Collections.Generic;
using System.Linq;

namespace Simplify.Trade
{
    public class ClosedAssetSummaryBook : List<SquarableStatement>
    {
        public double Profit {
            get
            {
                var profit = this.Sum(x => x.SaleValue - x.PurchaseValue);
                return profit;
            }
        }
    }
}