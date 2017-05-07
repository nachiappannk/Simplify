using System;
using System.Linq;

namespace Simplify.Books
{
    public class SquaredStatement
    {
        public SquaredStatement(TradeStatement ts1, TradeStatement ts2)
        {
            TradeStatements = new TradeStatement[] {ts1, ts2};
        }
        public TradeStatement[] TradeStatements { get; private set; }
    }

    public static class SquaredStatementExtns
    {
        private static TradeStatement GetPurchaseTradeStatement(this SquaredStatement squaredStatement)
        {
            return squaredStatement.TradeStatements.FirstOrDefault(x => x.IsPurchase);
        }

        private static TradeStatement GetSaleTradeStatement(this SquaredStatement squaredStatement)
        {
            return squaredStatement.TradeStatements.FirstOrDefault(x => !x.IsPurchase);
        }

        public static DateTime GetSaleDate(this SquaredStatement squaredStatement)
        {
            return squaredStatement.GetSaleTradeStatement().Date;
        }

        public static DateTime GetPurchaseDate(this SquaredStatement squaredStatement)
        {
            return squaredStatement.GetPurchaseTradeStatement().Date;
        }

        public static double GetQuantity(this SquaredStatement squaredStatement)
        {
            return squaredStatement.GetPurchaseTradeStatement().Quantity;
        }

        public static double GetProfit(this SquaredStatement squaredStatement)
        {
            return squaredStatement.GetSaleTradeStatement().Value - squaredStatement.GetPurchaseTradeStatement().Value;
        }

        public static double GetCost(this SquaredStatement squaredStatement)
        {
            return squaredStatement.GetPurchaseTradeStatement().Value;
        }

        public static double GetSale(this SquaredStatement squaredStatement)
        {
            return squaredStatement.GetSaleTradeStatement().Value;
        }

        private static string AppendString(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1)) s1 = "[NA]";
            if (string.IsNullOrEmpty(s2)) s2 = "[NA]";
            return s1 + " / " + s2;
        }

        public static string GetStt(this SquaredStatement squaredStatement)
        {
            return AppendString(squaredStatement.GetSaleTradeStatement().Stt,
                squaredStatement.GetPurchaseTradeStatement().Stt);
        }

        public static string GetAccount(this SquaredStatement squaredStatement)
        {
            return AppendString(squaredStatement.GetSaleTradeStatement().Account,
                squaredStatement.GetPurchaseTradeStatement().Account);
        }

        public static string GetContract(this SquaredStatement squaredStatement)
        {
            return AppendString(squaredStatement.GetSaleTradeStatement().Contract,
                squaredStatement.GetPurchaseTradeStatement().Contract);
        }

        public static string GetItemType(this SquaredStatement squaredStatement)
        {
            return squaredStatement.GetSaleTradeStatement().ItemType;
        }

        public static string GetName(this SquaredStatement squaredStatement)
        {
            return squaredStatement.GetSaleTradeStatement().Name;
        }

        public static int GetHoldingDays(this SquaredStatement squaredStatement)
        {
            return (squaredStatement.GetSaleDate() - squaredStatement.GetPurchaseDate()).Days;
        }
    }
}