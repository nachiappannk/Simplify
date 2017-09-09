using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            new List<string>() {"Item Type" },
            new List<string>() {"Name" },
            new List<string>() { "Account"},
            new List<string>(){ "Contract"} ,
            new List<string>(){ "Stt"},
            new List<string>(){ "Quantity"},
            new List<string>(){"Cost"},
            new List<string>(){ "Sale"}
        };

        private const int SerialNumber = 0;
        private const int Date = 1;
        private const int ItemType = 2;
        private const int Name = 3;
        private const int Account = 4;
        private const int Contract = 5;
        private const int Stt = 6;
        private const int Quantity = 7;
        private const int Cost = 8;
        private const int Sale = 9;

        public TradeLogGateway(string excelFileName)
        {
            _excelFileName = excelFileName;

        }
        
        public void WriteOpenPositions(IList<TradeStatement> balanceSheet)
        {
            var index = 0;
            using (var writer = new ExcelWriter(_excelFileName, "OpenPositions"))
            {
                writer.Write(index++, _columnsHeadingOptions.ToArray<object>());
                writer.SetColumnsWidth(6, 12,8, 30, 16,16,16, 12, 12, 12);
                writer.ApplyHeadingFormat(_columnsHeadingOptions.Count);
                writer.WriteList(index, balanceSheet, (b, rowIndex) => new object[]
                {
                    rowIndex - 1,
                    b.Date,
                    b.ItemType,
                    b.Name,
                    b.Account,
                    b.Contract,
                    b.Stt,
                    b.Quantity,
                    b.IsPurchase? b.Value: 0,
                    (!b.IsPurchase)? b.Value: 0,
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
                        ItemType = r.ReadString(ItemType),
                        Name = r.ReadString(Name),
                        Account = r.ReadString(Account),
                        Contract = r.ReadString(Contract),
                        Stt = r.ReadString(Stt),
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
