using System;
using System.Linq;

namespace Simplify.Trade
{
    public class SquarableStatement
    {
        public SquarableStatement(TradeStatement statement1, TradeStatement statement2)
        {
            IsSquared = true;
            TradeStatement purchaseTradedStatement = statement1.IsPurchase? statement1 : statement2;
            TradeStatement saleTradedStatement = statement1.IsPurchase ? statement2 : statement1;
            InitializeCommonProperties(purchaseTradedStatement);
            InitializePurchaseProperties(purchaseTradedStatement);
            InitializeSaleProperties(saleTradedStatement);
        }

        public SquarableStatement(TradeStatement tradeStatement)
        {
            IsSquared = false;
            InitializeCommonProperties(tradeStatement);
            if (tradeStatement.IsPurchase) InitializePurchaseProperties(tradeStatement);
            else InitializeSaleProperties(tradeStatement);
        }

        private void InitializeCommonProperties(TradeStatement purchaseTradedStatement)
        {
            Name = purchaseTradedStatement.Name;
            Quantity = purchaseTradedStatement.Quantity;
        }

        private void InitializePurchaseProperties(TradeStatement purchaseTradedStatement)
        {
            PurchaseDate = purchaseTradedStatement.Date;
            PurchaseValue = purchaseTradedStatement.Value;
            PurchaseTransactionTax = purchaseTradedStatement.TransactionTax;
            PurchaseTransactionDetail = purchaseTradedStatement.TransactionDetail;
        }

        private void InitializeSaleProperties(TradeStatement saleTradedStatement)
        {

            SaleDate = saleTradedStatement.Date;
            SaleValue = saleTradedStatement.Value;
            SaleTransactionTax = saleTradedStatement.TransactionTax;
            SaleTransactionDetail = saleTradedStatement.TransactionDetail;
        }

        public string Name { get; set; }
        public double Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public double PurchaseValue { get; set; }
        public string PurchaseTransactionTax { get; set; }
        public string PurchaseTransactionDetail { get; set; }
        public DateTime SaleDate { get; set; }
        public double SaleValue { get; set; }
        public string SaleTransactionTax { get; set; }
        public string SaleTransactionDetail { get; set; }
        public bool IsSquared { get; set; }
    }

    public static class DealExtentions
    {
        public static double GetProfit(this SquarableStatement squarableStatement)
        {
            return squarableStatement.SaleValue - squarableStatement.PurchaseValue;
        }

        public static double GetSalePerUnit(this SquarableStatement squarableStatement)
        {
            return squarableStatement.SaleValue / squarableStatement.Quantity;
        }

        public static double GetCostPerUnit(this SquarableStatement squarableStatement)
        {
            return squarableStatement.PurchaseValue / squarableStatement.Quantity;
        }

        public static int GetNumberOfHoldingDays(this SquarableStatement squarableStatement)
        {
            var result = squarableStatement.SaleDate - squarableStatement.PurchaseDate;
            return (int)result.TotalDays;
        }

        public static string GetOverallTransactionTax(this SquarableStatement squarableStatement)
        {
            var result = squarableStatement.SaleTransactionTax + " - "+ squarableStatement.PurchaseTransactionTax;
            return result;
        }

        public static string GetOverallTransactionDetail(this SquarableStatement squarableStatement)
        {
            var result = squarableStatement.SaleTransactionDetail + " - " + squarableStatement.PurchaseTransactionDetail;
            return result;
        }
    }
}