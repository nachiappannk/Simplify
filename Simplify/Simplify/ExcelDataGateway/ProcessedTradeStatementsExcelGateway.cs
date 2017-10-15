using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplify.Application;
using Simplify.Books;

namespace Simplify.ExcelDataGateway
{
    public class ProcessedTradeStatementsExcelGateway
    {
        private class AssetNameRecord
        {
            [ExcelColumn(1, "Name", 30)]
            public string Name { get; set; }
        }

        private class ProfitBookRecord
        {
            [ExcelColumn(1, "S.No.", 6)]
            public string SerialNumber { get; set; }

            [ExcelColumn(2, "Sale Date", 12)]
            public DateTime SaleDate { get; set; }

            [ExcelColumn(3, "Purchase Date", 12)]
            public DateTime PurchaseDate { get; set; }

            [ExcelColumn(4, "Holding Days", 8)]
            public int HoldingDays { get; set; }

            [ExcelColumn(5, "Name", 30)]
            public string Name { get; set; }

            [ExcelColumn(6, "Transaction Detail", 16)]
            public string TransactionDetail { get; set; }

            [ExcelColumn(7, "Transaction Tax", 16)]
            public string TransactionTax { get; set; }

            [ExcelColumn(8, "Quantity", 12)]
            public double Quantity { get; set; }

            [ExcelColumn(9, "Sale", 12)]
            public double Sale { get; set; }

            [ExcelColumn(10, "Cost", 12)]
            public double Cost { get; set; }

            [ExcelColumn(11, "Profit", 12)]
            public double Profit { get; set; }

        }

        private class SummaryRecord
        {
            

            [ExcelColumn(1, "Name", 30)]
            public string Name { get; set; }

            [ExcelColumn(2, "Quantity", 12)]
            public double Quantity { get; set; }

            [ExcelColumn(3, "Purchase Date", 16)]
            public DateTime PurchaseDate { get; set; }

            [ExcelColumn(4, "Cost", 16)]
            public double Cost { get; set; }

            [ExcelColumn(5, "Sale Date", 16)] 
            public Object SaleDate { get; set; } = string.Empty;

            [ExcelColumn(6, "Sale", 16)]
            public object Sale { get; set; } = string.Empty;

            [ExcelColumn(7, "Profit", 16)]
            public object Profit { get; set; } = string.Empty;
        }

        private class TradeRecord
        {
            [ExcelColumn(1, "S.No.", 6)]
            public int SerialNumber { get; set; }

            [ExcelColumn(2, "Date", 12)]
            public DateTime Date { get; set; }

            [ExcelColumn(3, "Name", 30)]
            public string Name { get; set; }

            [ExcelColumn(4, "Transaction Detail", 16)]
            public string TransactionDetail { get; set; }

            [ExcelColumn(5, "Transaction Tax", 16)]
            public string TransactionTax { get; set; }

            [ExcelColumn(6, "Quantity", 12)]
            public double Quantity { get; set; }

            [ExcelColumn(7, "Cost", 12)]
            public double Cost { get; set; }

            [ExcelColumn(8, "Sale", 12)]
            public double Sale { get; set; }
        }

        public class EffectiveCostRecord
        {
            [ExcelColumn(1, "Name", 30)]
            public string Name { get; set; }

            [ExcelColumn(2, "Average Effective Cost", 16)]
            public double Cost { get; set; }
        }


        public void Write(string fileName, ProcessedTradeStatementsContainer container)
        {
            ExcelWriter writer = new ExcelWriter(fileName);
            WriteAssetNames(container, writer);
            WriteSumary(container, writer);
            WriteProfitBook(container, writer);
            WriteOpenPositions(container, writer);
            WriteEffectiveCostBook(container, writer);
        }

        private static void WriteEffectiveCostBook(ProcessedTradeStatementsContainer container, ExcelWriter writer)
        {
            var effectiveCostStatements = container.EffectiveCostStatementBook
                .Select(x => new EffectiveCostRecord {Name = x.Name, Cost = x.AverageCost}).ToList();
            writer.AddSheet("EffectiveCost", effectiveCostStatements);
        }

        private static void WriteOpenPositions(ProcessedTradeStatementsContainer container, ExcelWriter writer)
        {
            var openPositionRecords = container.OpenPositionBook.Select((x, i) =>
            {
                var record = new TradeRecord
                {
                    SerialNumber = i + 1,
                    Date = x.Date,
                    Name = x.Name,
                    TransactionDetail = x.TransactionDetail,
                    TransactionTax = x.TransactionTax,
                    Quantity = x.Quantity,
                    Cost = x.IsPurchase ? x.Value : 0,
                    Sale = x.IsPurchase ? 0 : x.Value
                };
                return record;
            }).ToList();
            writer.AddSheet("OpenPosition", openPositionRecords);
        }

        private static void WriteProfitBook(ProcessedTradeStatementsContainer container, ExcelWriter writer)
        {
            var profitBookRecords = container.ProfitBook.Select((x, i) => new ProfitBookRecord
            {
                SerialNumber = (i + 1).ToString(),
                SaleDate = x.SaleDate,
                PurchaseDate = x.PurchaseDate,
                HoldingDays = x.GetNumberOfHoldingDays(),
                Name = x.Name,
                TransactionTax = x.GetOverallTransactionTax(),
                TransactionDetail = x.GetOverallTransactionDetail(),
                Quantity = x.Quantity,
                Sale = x.SaleValue,
                Cost = x.PurchaseValue,
                Profit = x.GetProfit(),
            }).ToList();

            writer.AddSheet("ProfitBook", profitBookRecords);
        }

        private static void WriteSumary(ProcessedTradeStatementsContainer container, ExcelWriter writer)
        {
            List<SquarableStatement> summaryStatements = new List<SquarableStatement>();
            foreach (var namedBook in container.AssetSummaryBooks)
            foreach (var statments in namedBook.Values)
                summaryStatements.AddRange(statments);

            var summaryRecords = summaryStatements.Select(x =>
            {
                var record = new SummaryRecord
                {
                    Name = x.Name,
                    PurchaseDate = x.PurchaseDate,
                    Cost = x.PurchaseValue,
                    Quantity = x.Quantity
                };
                if (x.IsSquared) record.SaleDate = x.SaleDate;
                if (x.IsSquared) record.Sale = x.SaleValue;
                if (x.IsSquared) record.Profit = x.GetProfit();

                return record;
            }).ToList();
            writer.AddSheet("Summary", summaryRecords);
        }

        private static void WriteAssetNames(ProcessedTradeStatementsContainer container, ExcelWriter writer)
        {
            var assetNameRecords = container.AssetNamesBook.Select(x => new AssetNameRecord {Name = x}).ToList();
            writer.AddSheet("AssetName", assetNameRecords);
        }
    }
}
