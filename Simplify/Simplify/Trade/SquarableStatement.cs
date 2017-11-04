using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Simplify.Trade
{
    public class SquarableStatement
    {
        public SquarableStatement(TradeStatement statement1, TradeStatement statement2)
        {
            IsSquared = true;
            TransactionType = TransactionType.Squared;
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
            if (tradeStatement.IsPurchase)
            {
                InitializePurchaseProperties(tradeStatement);
                TransactionType = TransactionType.Purchase;
            }
            else
            {
                InitializeSaleProperties(tradeStatement);
                TransactionType = TransactionType.Sale;
            }
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

        public TransactionType TransactionType { get; set; }
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

    public enum TransactionType
    {
        Purchase,
        Sale,
        Squared,
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

    public static class SquarableStatementExtentions
    {
        public static TradeStatement ConvertToTradeStatement(this SquarableStatement squarableStatement)
        {
            if (squarableStatement.IsSquared) throw new Exception();
            var tradeStatement = new TradeStatement();
            tradeStatement.Name = squarableStatement.Name;
            tradeStatement.SerialNumber = 0;
            tradeStatement.Quantity = squarableStatement.Quantity;
            if (squarableStatement.TransactionType == TransactionType.Purchase)
            {
                tradeStatement.Date = squarableStatement.PurchaseDate;
                tradeStatement.Value = squarableStatement.PurchaseValue;
                tradeStatement.IsPurchase = true;
                tradeStatement.TransactionDetail = squarableStatement.PurchaseTransactionDetail;
                tradeStatement.TransactionTax = squarableStatement.PurchaseTransactionTax;
            }
            else
            {
                tradeStatement.Date = squarableStatement.SaleDate;
                tradeStatement.Value = squarableStatement.SaleValue;
                tradeStatement.IsPurchase = false;
                tradeStatement.TransactionDetail = squarableStatement.SaleTransactionDetail;
                tradeStatement.TransactionTax = squarableStatement.SaleTransactionTax;
            }
            return tradeStatement;
        }
    }

}