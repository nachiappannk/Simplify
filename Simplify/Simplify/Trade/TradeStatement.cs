using System;

namespace Simplify.Trade
{
    public class TradeStatement
    {
        public int SerialNumber { get; set; }
        public DateTime Date { get; set; }
        public bool IsPurchase { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }
        public string TransactionTax { get; set; }
        public string TransactionDetail { get; set; }
    }

    public static class TradeStatementExtentions
    {
        public static double GetAverageValue(this TradeStatement tradeStatement)
        {
            return tradeStatement.Value / tradeStatement.Quantity;
        }
    }
}
