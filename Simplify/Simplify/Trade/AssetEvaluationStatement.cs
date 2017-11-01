using System;
using System.ComponentModel;
using Simplify.CommonDefinitions;

namespace Simplify.Trade
{
    public class AssetEvaluationStatement
    {
        private readonly Quote _quote;

        public AssetEvaluationStatement(Quote quote)
        {
            _quote = quote;
            SetCurrentValuePerUnit(quote);
            quote.Changed += () => SetCurrentValuePerUnit(quote);
        }

        private void SetCurrentValuePerUnit(Quote quote)
        {
            CurrentValuePerUnit = quote.QuotedValue;
        }

        public event Action Changed;
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }
        public string TransactionTax { get; set; }
        public string TransactionDetail { get; set; }


        private double? _currentValuePerUnit;
        public double? CurrentValuePerUnit
        {
            get { return _currentValuePerUnit; }
            set
            {
                if (_currentValuePerUnit.IsNullableDoubleEqual(value)) return;
                _currentValuePerUnit = value;
                _quote.QuotedValue = _currentValuePerUnit;
                Changed?.Invoke();
            }
        }
    }

    public static class AssetEvaluationStatementExtentions
    {
        public static void InitializeFromTradeStatement(this AssetEvaluationStatement assetEvaluationStatement,
            TradeStatement openPosition)
        {
            assetEvaluationStatement.Name = openPosition.Name;
            assetEvaluationStatement.Date = openPosition.Date;
            assetEvaluationStatement.Quantity = openPosition.Quantity;
            assetEvaluationStatement.TransactionDetail = openPosition.TransactionDetail;
            assetEvaluationStatement.TransactionTax = openPosition.TransactionTax;
            assetEvaluationStatement.Value = openPosition.Value;
        }

        public static double? GetUnrealizedProfit(this AssetEvaluationStatement statement)
        {
            if (statement.CurrentValuePerUnit == null) return null;
            return statement.GetCurrentValue() - statement.Value;
        }

        public static double? GetCurrentValue(this AssetEvaluationStatement statement)
        {
            if (statement.CurrentValuePerUnit == null) return null;
            return statement.CurrentValuePerUnit * statement.Quantity;
        }

        public static double GetValuePerUnit(this AssetEvaluationStatement statement)
        {
            return statement.Value / statement.Quantity;
        }

    }
}