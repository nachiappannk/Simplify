using System;

namespace Simplify.Trade
{
    public class PurchasedAssetEvaluationStatement
    {
        public event Action EvaluationChanged;
        private readonly Quote _quote;

        public PurchasedAssetEvaluationStatement(Quote quote)
        {
            _quote = quote;
            _quote.Changed += () => { EvaluationChanged?.Invoke(); };
        }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }
        public string TransactionTax { get; set; }
        public string TransactionDetail { get; set; }
        public double? QuotePerUnit
        {
            get { return _quote.QuotedValue; }
            set { _quote.QuotedValue = value; } 
        }
    }

    public static class PurchasedAssetEvaluationStatementExtentions
    {
        public static PurchasedAssetEvaluationStatement CreatePurchasedAssetEvalutionStatement(
            this SquarableStatement statement, Quote quote)
        {
            if(statement.TransactionType != TransactionType.Purchase) throw new Exception();
            var result = new PurchasedAssetEvaluationStatement(quote)
            {
                Name = statement.Name,
                Date = statement.PurchaseDate,
                Quantity = statement.Quantity,
                TransactionDetail = statement.PurchaseTransactionDetail,
                TransactionTax = statement.PurchaseTransactionTax,
                Value = statement.PurchaseValue
            };
            return result;
        }

        public static double GetAverageValue(this PurchasedAssetEvaluationStatement statement)
        {
            return statement.Value / statement.Quantity;
        }

        public static double? GetCurrentValue(this PurchasedAssetEvaluationStatement statement)
        {
            if (!statement.QuotePerUnit.HasValue) return null;
            return statement.QuotePerUnit.Value * statement.Quantity;
        }

        public static double? GetUnrealizedProfit(this PurchasedAssetEvaluationStatement statement)
        {
            if (!statement.QuotePerUnit.HasValue) return null;
            return statement.GetCurrentValue() - statement.Value;
        }
    }
}