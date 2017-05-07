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
        readonly string[] headings = { "S.No.", "Date","Item Type", "Name", "Account", "Contract" ,"Stt", "Quantity", "Cost", "Sale"};
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
                writer.Write(index++, headings.ToArray<object>());
                writer.SetColumnsWidth(6, 12,8, 30, 16,16,16, 12, 12, 12);
                writer.ApplyHeadingFormat(headings.Length);
                writer.WriteList(index, balanceSheet, (b, rowIndex) => new object[]
                {
                    b.SerialNumber,
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
            using (ExcelReader reader = new ExcelReader(_excelFileName, sheetName))
            {
                SheetHeadingLogger.LogHeadingRowDetails(logger, reader, headings);
                var tradeStatements = reader.ReadAllLines(1, (r) =>
                {
                    var cost = r.ReadDouble(Cost);
                    var sale = r.ReadDouble(Sale);
                    var isPurchase = false;
                    if (cost > 0.0001 && sale > 0.0001)
                    {
                        logger.Error("Both cost and sale has values");
                    }
                    if (cost > sale)
                        isPurchase = true;
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
