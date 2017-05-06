using System;
using System.Linq;
using Simplify.Books;

namespace Simplify.ExcelDataGateway
{
    public class BalanceSheetGateway
    {
        private readonly string _excelFileName;
        readonly string[] headings = { "S.No.", "Ledger", "Credit", "Debit", "Total" };
        private const int SerialNumber = 0;
        private const int Ledger = 1;
        private const int Credit = 2;
        private const int Debit = 3;
        private const int Total = 4;

        public BalanceSheetGateway(string excelFileName)
        {
            _excelFileName = excelFileName;

        }

        public void WriteBalanceSheet(BalanceSheetBook balanceSheet)
        {
            var bSheetCopy = balanceSheet.Where(s => Math.Abs((double) s.Value) > 0.001).ToList();
            var index = 0;
            using (var writer = new ExcelWriter(_excelFileName, "BS"))
            {
                writer.Write(index++, headings.ToArray<object>());
                writer.SetColumnsWidth(6, 45, 12, 12, 12);
                writer.ApplyHeadingFormat(headings.Length);
                writer.WriteList(index, bSheetCopy, (b, rowIndex) => new object[]
                {
                    rowIndex - 1,
                    b.Name,
                    b.GetCreditValue(),
                    b.GetDebitValue(),
                });
                index = index + bSheetCopy.Count;
                writer.Write(index, "", "Total", bSheetCopy.GetCreditTotal(), bSheetCopy.GetDebitTotal(), bSheetCopy.GetTotal());

            }
        }

        public BalanceSheetBook GetBalanceSheet(ILogger logger, string sheetName)
        {
            using (ExcelReader reader = new ExcelReader(_excelFileName, sheetName))
            {
                SheetHeadingLogger.LogHeadingRowDetails(logger, reader, headings);
                var balanceSheetStatements = reader.ReadAllLines(1, (r) =>
                {
                    var credit = r.ReadDouble(Credit);
                    var debit = r.ReadDouble(Debit);
                    if (credit > 0.001 && debit > 0.001)
                    {
                        logger.Error("Both credit and debit has values");
                    }
                    return new Statement()
                    {
                        Name = r.ReadString(Ledger),
                        Value = credit - debit,
                    };
                }).ToList();
                var balanceSheet = new BalanceSheetBook();
                balanceSheet.AddRange(balanceSheetStatements);
                return balanceSheet;
            }
        }

    }
}