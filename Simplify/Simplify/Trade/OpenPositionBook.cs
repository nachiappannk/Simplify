using System.Collections.Generic;

namespace Simplify.Trade
{
    public class OpenPositionBook
    {
        public List<TradeStatement> OpenPositionRecords { get; set; }

        public double CostOfOpenPosition { get; set; }

    }
}