using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace Simplify.Books
{
    public class Deal
    {
        public Deal(TradeStatement[] statements)
        {
            TradeStatement purchaseTradedStatement = statements.First(x => x.IsPurchase);
            TradeStatement saleTradedStatement = statements.First(x => !x.IsPurchase);
            IsDealClosed = true;
            InitializeCommonAndPurchaseProperties(purchaseTradedStatement);
            InitializeSaleProperties(saleTradedStatement);
        }

        public Deal(TradeStatement purchaseTradedStatement)
        {
            IsDealClosed = false;
            InitializeCommonAndPurchaseProperties(purchaseTradedStatement);
        }

        private void InitializeSaleProperties(TradeStatement saleTradedStatement)
        {
            
            SaleDate = saleTradedStatement.Date;
            SaleValue = saleTradedStatement.Value;
            SaleTransactionTax = saleTradedStatement.TransactionTax;
            SaleTransactionDetail = saleTradedStatement.TransactionDetail;
        }

        private void InitializeCommonAndPurchaseProperties(TradeStatement purchaseTradedStatement)
        {
            Name = purchaseTradedStatement.Name;
            Quantity = purchaseTradedStatement.Quantity;
            PurchaseDate = purchaseTradedStatement.Date;
            PurchaseValue = purchaseTradedStatement.Value;
            PurchaseTransactionTax = purchaseTradedStatement.TransactionTax;
            PurchaseTransactionDetail = purchaseTradedStatement.TransactionDetail;
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
        public bool IsDealClosed { get; set; }
    }

    public static class DealExtentions
    {
        public static double GetProfit(this Deal deal)
        {
            return deal.SaleValue - deal.PurchaseValue;
        }

        public static int GetNumberOfHoldingDays(this Deal deal)
        {
            var result = deal.SaleDate - deal.PurchaseDate;
            return (int)result.TotalDays;
        }

        public static string GetOverallTransactionTax(this Deal deal)
        {
            var result = deal.SaleTransactionTax + " - "+ deal.PurchaseTransactionTax;
            return result;
        }

        public static string GetOverallTransactionDetail(this Deal deal)
        {
            var result = deal.SaleTransactionDetail + " - " + deal.PurchaseTransactionDetail;
            return result;
        }
    }
}