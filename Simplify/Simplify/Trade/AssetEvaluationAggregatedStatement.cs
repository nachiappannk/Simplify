using System;
using Simplify.CommonDefinitions;

namespace Simplify.Trade
{
    public class AssetEvaluationAggregatedStatement
    {
        private readonly Quote _quote;

        public AssetEvaluationAggregatedStatement(Quote quote)
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
        public DateTime PurchaseStartDate { get; set; }
        public DateTime PurchaseEndDate { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }

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


    public static class AssetEvaluationAggregatedStatementExtentions
    {
        public static double? GetUnrealizedProfit(this AssetEvaluationAggregatedStatement statement)
        {
            if (statement.CurrentValuePerUnit == null) return null;
            return statement.GetCurrentValue() - statement.Value;
        }

        public static double? GetCurrentValue(this AssetEvaluationAggregatedStatement statement)
        {
            if (statement.CurrentValuePerUnit == null) return null;
            return statement.CurrentValuePerUnit * statement.Quantity;
        }

        public static double GetValuePerUnit(this AssetEvaluationAggregatedStatement statement)
        {
            return statement.Value / statement.Quantity;
        }
    }
}