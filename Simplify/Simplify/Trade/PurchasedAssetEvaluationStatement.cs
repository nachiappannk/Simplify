using System;

namespace Simplify.Trade
{
    public class PurchasedAssetEvaluationStatement
    {
        public event Action Changed;
        private readonly Quote _quote;

        public PurchasedAssetEvaluationStatement(Quote quote)
        {
            _quote = quote;
            _quote.Changed += () => { Changed?.Invoke(); };
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
    }
}