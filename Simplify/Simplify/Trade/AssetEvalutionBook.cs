using System.Collections.Generic;

namespace Simplify.Trade
{
    public class AssetEvalutionBook
    {
        public List<AssetEvaluationStatement> Statements { get; set; }
        public double TotalCostOfOpenPosition { get; set; }

        public double? CurrentValueOfOpenPosition { get; set; }

        public double? UnrealizedProfit { get; set; }
    }
}