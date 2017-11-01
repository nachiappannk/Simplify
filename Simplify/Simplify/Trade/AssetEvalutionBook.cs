using System.Collections.Generic;
using System.Linq;

namespace Simplify.Trade
{
    public class AssetEvalutionBook
    {
        public List<AssetEvaluationStatement> Statements { get; set; }

        public double TotalCostOfOpenPosition
        {
            get { return Statements.Sum(x => x.Value); }
        }

        public double? CurrentValueOfOpenPosition
        {
            get
            {
                var isAnyCurrentValueNotAvailable = Statements.Any(x => x.CurrentValuePerUnit == null);
                if (isAnyCurrentValueNotAvailable) return null;
                return Statements.Sum(x => x.GetCurrentValue());
            }
        }

        public double? UnrealizedProfit
        {
            get
            {
                var isAnyCurrentValueNotAvailable = Statements.Any(x => x.CurrentValuePerUnit == null);
                if (isAnyCurrentValueNotAvailable) return null;
                return Statements.Sum(x => x.GetUnrealizedProfit());
            }
        }
    }
}