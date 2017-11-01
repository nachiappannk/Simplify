using System.Collections.Generic;
using System.Linq;

namespace Simplify.Trade
{
    public class OpenAssetSummaryBook
    {
        public List<SquarableStatement> Statements;

        public double QuanityOfOpenPosition
        {
            get
            {
                var openPositions = Statements.Where(x => x.IsSquared == false);
                return openPositions.Sum(x => x.Quantity);
            }
        }

        public double AverageCost
        {
            get
            {
                var openPositions = Statements.Where(x => x.IsSquared == false).ToList();
                var quantity = openPositions.Sum(x => x.Quantity);
                var value = openPositions.Sum(x => x.PurchaseValue);
                return value / quantity;
            }
        }

        public double OpenPositionCost
        {
            get
            {
                var openPositions = Statements.Where(x => x.IsSquared == false).ToList();
                return  openPositions.Sum(x => x.PurchaseValue);
            }
        }

        public double RealizedProfit
        {
            get
            {
                var closedPartOfStatement = Statements.Where(x => x.IsSquared).ToList();
                return closedPartOfStatement.Sum(x => x.SaleValue - x.PurchaseValue);
            }
        }

        
    }
}