using System;

namespace Simplify.Trade
{
    public class PurchasedAssetEvaluationSummarizedStatement
    {
        private readonly Quote _quote;
        public event Action EvaluationChanged;

        public PurchasedAssetEvaluationSummarizedStatement(Quote quote)
        {
            _quote = quote;
            _quote.Changed += () => { EvaluationChanged?.Invoke(); };
        }

        public string Name { get; set; }
        public DateTime PurchaseStartDate { get; set; }
        public DateTime PurchaseEndDate { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }

        public double? QuotePerUnit
        {
            get { return _quote.QuotedValue; }
            set { _quote.QuotedValue = value; }
        }
    }

    

    public static class PurchasedAssetEvaluationSummarizedStatementExtentions
    {
        public static double GetAverageValue(this PurchasedAssetEvaluationSummarizedStatement statement)
        {
            return statement.Value / statement.Quantity;
        }

        public static double? GetCurrentValue(this PurchasedAssetEvaluationSummarizedStatement statement)
        {
            if (!statement.QuotePerUnit.HasValue) return null;
            return statement.QuotePerUnit.Value * statement.Quantity;
        }

        public static double? GetUnrealizedProfit(this PurchasedAssetEvaluationSummarizedStatement statement)
        {
            if (!statement.QuotePerUnit.HasValue) return null;
            return statement.GetCurrentValue() - statement.Value;
        }
    }
}