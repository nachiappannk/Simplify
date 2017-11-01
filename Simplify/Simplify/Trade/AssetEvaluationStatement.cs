﻿using System;

namespace Simplify.Trade
{
    public class AssetEvaluationStatement
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }
        public string TransactionTax { get; set; }
        public string TransactionDetail { get; set; }
        public double? CurrentValuePerUnit { get; set; }
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