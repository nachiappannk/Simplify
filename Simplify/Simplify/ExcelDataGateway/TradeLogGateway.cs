using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplify.Application;
using Simplify.Books;

namespace Simplify.ExcelDataGateway
{
    public class TradeLogGateway
    {
        private readonly string _excelFileName;
        readonly List<List<string>> _columnsHeadingOptions = new List<List<string>>()
        {
            new List<string>() { "S.No."},
            new List<string>() {"Date" },
            new List<string>() {"Name" },
            new List<string>() { "Transaction Detail"},
            new List<string>(){ "Transaction Tax"} ,
            new List<string>(){ "Quantity"},
            new List<string>(){"Cost"},
            new List<string>(){ "Sale"}
        };

        private const int SerialNumber = 0;
        private const int Date = 1;
        private const int Name = 2;
        private const int TransactionDetail = 3;
        private const int TransactionTax = 4;
        private const int Quantity = 5;
        private const int Cost = 6;
        private const int Sale = 7;

        public TradeLogGateway(string excelFileName)
        {
            _excelFileName = excelFileName;

        }
        
        public void WriteSummary(IList<SquarableStatement> deals)
        {
            var index = 0;

            List<List<string>> columnsHeadingOptions = new List<List<string>>()
            {
                new List<string>() {"Name" },
                new List<string>(){ "Quantity"},
                new List<string>() {"Purchase Date" },
                new List<string>(){"Cost"},
                new List<string>() {"Sale Date" },
                new List<string>(){ "Sale"},
                new List<string>(){ "Profit"}
            };

            
        var dealsSorted = deals.OrderBy(x => x.PurchaseDate).ToList();

            using (var writer = new ExcelWriter(_excelFileName, "Summary"))
            {
                writer.Write(index++, columnsHeadingOptions.ToArray<object>());
                writer.SetColumnsWidth(30, 12, 16,16, 12, 12,12);
                writer.ApplyHeadingFormat(columnsHeadingOptions.Count);
                writer.WriteList(index, dealsSorted,
                    (b, rowIndex) =>
                    {
                        if(b.IsSquared)
                        return new object[]
                        {
                            b.Name,
                            b.Quantity,
                            b.PurchaseDate,
                            b.PurchaseValue,
                            b.SaleDate,
                            b.SaleValue,
                            b.SaleValue - b.PurchaseValue
                        };
                        else
                            return new object[]
                            {
                                b.Name,
                                b.Quantity,
                                b.PurchaseDate,
                                b.PurchaseValue,
                            };
                    });
            }
        }

        private string SortKey(SquarableStatement squarableStatement)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(squarableStatement.Name);
            builder.Append(squarableStatement.IsSquared ? "0" : "1");
            builder.Append(squarableStatement.SaleDate.Year);
            builder.Append("_");
            builder.Append(squarableStatement.SaleDate.Month);
            builder.Append("_");
            builder.Append(squarableStatement.SaleDate.Day);

            return builder.ToString();
        }

        public void WriteOpenPositions(IList<SquarableStatement> deals)
        {
            var index = 0;

            var dealsSorted = deals.OrderBy(SortKey).ToList();

            using (var writer = new ExcelWriter(_excelFileName, "OpenPositions"))
            {
                var comlumnsHeadingOptions = _columnsHeadingOptions.ToList();
                comlumnsHeadingOptions.RemoveAt(comlumnsHeadingOptions.Count - 1);
                writer.Write(index++, comlumnsHeadingOptions.ToArray<object>());
                writer.SetColumnsWidth(6, 12, 30, 16, 16, 12, 12);
                writer.ApplyHeadingFormat(_columnsHeadingOptions.Count);
                writer.WriteList(index, dealsSorted, (b, rowIndex) => new object[]
                {
                    rowIndex - 1,
                    b.PurchaseDate,
                    b.Name,
                    b.PurchaseTransactionDetail,
                    b.PurchaseTransactionTax,
                    b.Quantity,
                    b.PurchaseValue,
                });
            }
        }

        public List<TradeStatement> ReadTradeLog(ILogger logger, string sheetName)
        {
            using (ExcelReader reader = new ExcelReader(_excelFileName, sheetName, logger))
            {
                SheetHeadingVerifier.VerifyHeadingNames(logger, reader, _columnsHeadingOptions);
                var tradeStatements = reader.ReadAllLines(1, (r) =>
                {
                    var isCostAvailable = r.IsValueAvailable(Cost);
                    var cost = isCostAvailable ? r.ReadDouble(Cost) : 0;
                    var isSaleAvailable = r.IsValueAvailable(Sale);
                    var sale = isSaleAvailable ? r.ReadDouble(Sale) : 0;
                    if (isCostAvailable && isSaleAvailable)
                    {
                        logger.Log(MessageType.IgnorableError, $"In file{r.FileName}, " +
                                                               $"in sheet{r.SheetName}, " +
                                                               $"in line no. {r.LineNumber}, " +
                                                               "both cost and sale is mentioned. Taking the sum as value");
                    }

                    bool isPurchase = cost > sale;
                    if (cost < 0.001 && sale< 0.001)
                        isPurchase = true;//Bonus Case
                    var value = cost + sale;
                    return new TradeStatement()
                    {
                        SerialNumber = r.ReadInteger(SerialNumber),
                        Date = r.ReadDate(Date),
                        Name = r.ReadString(Name),
                        TransactionDetail = r.ReadString(TransactionDetail),
                        TransactionTax = r.ReadString(TransactionTax),
                        Quantity = r.ReadDouble(Quantity),
                        Value = value,
                        IsPurchase = isPurchase,
                    };
                }).ToList();
                return tradeStatements.ToList();
            }
        }
    }
}
