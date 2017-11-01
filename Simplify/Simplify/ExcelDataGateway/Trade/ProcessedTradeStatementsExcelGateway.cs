using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Application;
using Simplify.Trade;

namespace Simplify.ExcelDataGateway.Trade
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


            [ExcelColumn(5, "Average Cost", 16)]
            public double CostPerUnit { get; set; }

            [ExcelColumn(6, "Sale Date", 16)] 
            public Object SaleDate { get; set; } = string.Empty;

            [ExcelColumn(7, "Sale", 16)]
            public object Sale { get; set; } = string.Empty;

            [ExcelColumn(8, "Average Sale", 16)]
            public object SalePerUnit { get; set; } = string.Empty;

            [ExcelColumn(9, "Profit", 16)]
            public object Profit { get; set; } = string.Empty;
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
        }

        private void WriteOpenPositions(ProcessedTradeStatementsContainer container, ExcelWriter writer)
        {
            var tradeLogGateway = new TradeLogGateway();
            tradeLogGateway.WriteOpenPositions(container, writer);
        }

        public List<TradeStatement> ReadTradeLog(ILogger logger, string fileName, string sheetName)
        {
            var tradeLogGateway = new TradeLogGateway();
            return tradeLogGateway.ReadTradeLog(logger,fileName, sheetName);
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
            foreach (var namedBook in container.OpenAssetSummaryBooks)
                summaryStatements.AddRange(namedBook.Value.Statements);

            var summaryRecords = summaryStatements.Select(x =>
            {
                var record = new SummaryRecord
                {
                    Name = x.Name,
                    PurchaseDate = x.PurchaseDate,
                    Cost = x.PurchaseValue,
                    Quantity = x.Quantity,
                    CostPerUnit = x.GetCostPerUnit(),
                    
                };
                if (x.IsSquared) record.SaleDate = x.SaleDate;
                if (x.IsSquared) record.Sale = x.SaleValue;
                if (x.IsSquared) record.SalePerUnit = x.GetSalePerUnit();
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

    public class TradeLogGateway
    {
        private const string SerialNumberString = "S.No.";
        private const string DateString = "Date";
        private const string NameString = "Name";
        private const string TransactionDetailString = "Transaction Detail";
        private const string TransactionTaxString = "Transaction Tax";
        private const string QuantityString = "Quantity";
        private const string CostString = "Cost";
        private const string SaleString = "Sale";

        private static readonly List<string> ColumnNames = new List<string>
        {
            SerialNumberString,
            DateString,
            NameString,
            TransactionDetailString,
            TransactionTaxString,
            QuantityString,
            CostString,
            SaleString
        };


        private class TradeRecord
        {
            [ExcelColumn(1, SerialNumberString, 6)]
            public int SerialNumber { get; set; }

            [ExcelColumn(2, DateString, 12)]
            public DateTime Date { get; set; }

            [ExcelColumn(3, NameString, 30)]
            public string Name { get; set; }

            [ExcelColumn(4, TransactionDetailString, 16)]
            public string TransactionDetail { get; set; }

            [ExcelColumn(5, TransactionTaxString, 16)]
            public string TransactionTax { get; set; }

            [ExcelColumn(6, QuantityString, 12)]
            public double Quantity { get; set; }

            [ExcelColumn(7, CostString, 12)]
            public double Cost { get; set; }

            [ExcelColumn(8, SaleString, 12)]
            public double Sale { get; set; }
        }

        private int GetExcelIndex(string s)
        {
            return ColumnNames.IndexOf(s);
        }

        public void WriteOpenPositions(ProcessedTradeStatementsContainer container, ExcelWriter writer)
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


        public List<TradeStatement> ReadTradeLog(ILogger logger, string fileName, string sheetName)
        {
            using (ExcelReader reader = new ExcelReader(fileName, sheetName, logger))
            {
                SheetHeadingVerifier.VerifyHeadingNames(logger, reader, ColumnNames);
                var tradeStatements = reader.ReadAllLines(1, (r) =>
                {
                    var isCostAvailable = r.IsValueAvailable(GetExcelIndex(CostString));
                    var cost = isCostAvailable ? r.ReadDouble(GetExcelIndex(CostString)) : 0;
                    var isSaleAvailable = r.IsValueAvailable(GetExcelIndex(SaleString));
                    var sale = isSaleAvailable ? r.ReadDouble(GetExcelIndex(SaleString)) : 0;
                    if (isCostAvailable && isSaleAvailable)
                    {
                        logger.Log(MessageType.IgnorableError, $"In file{r.FileName}, " +
                                                               $"in sheet{r.SheetName}, " +
                                                               $"in line no. {r.LineNumber}, " +
                                                               "both cost and sale is mentioned. Taking the sum as value");
                    }

                    bool isPurchase = cost > sale;
                    if (cost < 0.001 && sale < 0.001)
                        isPurchase = true;//Bonus Case
                    var value = cost + sale;
                    return new TradeStatement()
                    {
                        SerialNumber = r.ReadInteger(GetExcelIndex(SerialNumberString)),
                        Date = r.ReadDate(GetExcelIndex(DateString)),
                        Name = r.ReadString(GetExcelIndex(NameString)),
                        TransactionDetail = r.ReadString(GetExcelIndex(TransactionDetailString)),
                        TransactionTax = r.ReadString(GetExcelIndex(TransactionTaxString)),
                        Quantity = r.ReadDouble(GetExcelIndex(QuantityString)),
                        Value = value,
                        IsPurchase = isPurchase,
                    };
                }).ToList();
                return tradeStatements.ToList();
            }
        }
    }

}
