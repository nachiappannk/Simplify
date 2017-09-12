using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Books;

namespace Simplify.ExcelDataGateway
{
    public class BalanceSheetGateway
    {
        private readonly string _excelFileName;

        private readonly List<List<string>> headings = new List<List<string>>()
        {
            new List<string>(){ "S.No."},
            new List<string>(){ "Ledger"},
            new List<string>(){ "Credit" },
            new List<string>(){ "Debit" },
            new List<string>(){ "Total" },

        };
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
            var index = 0;
            using (var writer = new ExcelWriter(_excelFileName, "BS"))
            {
                writer.Write(index++, headings.ToArray<object>());
                writer.SetColumnsWidth(6, 45, 12, 12, 12);
                writer.ApplyHeadingFormat(headings.Count);
                writer.WriteList(index, balanceSheet, (b, rowIndex) => new object[]
                {
                    rowIndex - 1,
                    b.Description,
                    b.GetCreditValue(),
                    b.GetDebitValue(),
                });
                index = index + balanceSheet.Count;
                writer.Write(index, "", "Total", balanceSheet.GetCreditTotal(), balanceSheet.GetDebitTotal(), 
                    balanceSheet.GetTotal());

            }
        }

        public BalanceSheetBook GetBalanceSheet(ILogger logger, string sheetName)
        {
            using (ExcelReader reader = new ExcelReader(_excelFileName, sheetName, logger))
            {
                SheetHeadingVerifier.VerifyHeadingNames(logger, reader, headings);
                var balanceSheetStatements = reader.ReadAllLines(1, (r) =>
                {
                    var isValid = r.IsValueAvailable(SerialNumber);
                    var isCreditAvailable = r.IsValueAvailable(Credit);
                    var credit = isCreditAvailable? r.ReadDouble(Credit) : 0;
                    var isDebitAvailable = r.IsValueAvailable(Debit);
                    var debit = isDebitAvailable ? r.ReadDouble(Debit) : 0;
                    if (isCreditAvailable && isDebitAvailable)
                    {
                        logger.Log(MessageType.IgnorableError, $"In file{r.FileName}, " +
                                                               $"in sheet{r.SheetName}, " +
                                                               $"in line no. {r.LineNumber}, " +
                                                               "both credit and debit is mentioned. Taking the difference as value");
                    }
                    if (!isCreditAvailable && !isDebitAvailable)
                    {
                        logger.Log(MessageType.IgnorableError, $"In file{r.FileName}, " +
                                                               $"in sheet{r.SheetName}, " +
                                                               $"in line no. {r.LineNumber}, " +
                                                               "both credit and debit is not mentioned. Taking the value as 0");
                    }
                    return new StatementWithValidity()
                    {
                        IsValid = isValid,
                        Description = r.ReadString(Ledger),
                        Value = credit - debit,
                    };
                }).ToList();
                var balanceSheet = new BalanceSheetBook();
                balanceSheet.AddRange(balanceSheetStatements.Where(x => x.IsValid).Select(y => new Statement() {Description = y.Description, Value = y.Value }));
                return balanceSheet;
            }
        }

        public class StatementWithValidity : Statement 
        {
            public bool IsValid { get; set; }
        }
    }
}