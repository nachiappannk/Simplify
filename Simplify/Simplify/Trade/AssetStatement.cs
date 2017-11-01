using System.Collections.Generic;

namespace Simplify.Trade
{
    public class AssetStatement
    {
        public string Name { get; set; }

        public double Value { get; set; }

        public double Quantity { get; set; }

        public double CurrentValue { get; set; }
    }


    public class AssetSummary
    {
        public List<AssetStatement> AssetStatements { get; set; }
    }
}