using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Simplify.Books;

namespace Simplify.ExcelDataGateway
{
    public class BooksOfAccountReader
    {
        private readonly string _excelFileName;
        private readonly string _sheetName;

        public BooksOfAccountReader(string excelFileName, string sheetName)
        {
            _excelFileName = excelFileName;
            _sheetName = sheetName;
        }

        private static void AssertSheetExists(string excelFileName, string sheetName)
        {
            ExcelSheetInfoProvider sheetInfoProvider = new ExcelSheetInfoProvider(excelFileName);
            if (!sheetInfoProvider.IsSheetPresent(sheetName))
            {
                throw new Exception(sheetName + ": sheet does not exist");
            }
        }

        public List<JournalStatement> GetJournalStatements(ILogger logger)
        {
            AssertSheetExists(_excelFileName, _sheetName);
            using (var package = ExcelSheetInfoProvider.GetReadOnlyExcelPackage(_excelFileName))
            {
                using (var sheet = package.Workbook.Worksheets[_sheetName])
                {
                    const int SerialNumber = 0;
                    const int Date = 1;
                    const int EntryType = 2;
                    const int Book = 3;
                    const int LedgerName = 4;
                    const int AdditionalInformation = 5;
                    const int Credit = 6;
                    const int Debit = 7;


                    ExcelReader reader = new ExcelReader();
                    int index = 0;
                    string[] headings = reader.ReadLine(sheet, index++, 
                        r => new string[]
                        {
                            r.ReadString(SerialNumber),
                            r.ReadString(Date),
                            r.ReadString(EntryType),
                            r.ReadString(Book),
                            r.ReadString(LedgerName),
                            r.ReadString(AdditionalInformation),
                            r.ReadString(Credit),
                            r.ReadString(Debit),
                        });
                    logger.Info("Read " + headings[SerialNumber] + " as " + nameof(SerialNumber));
                    logger.Info("Read " + headings[Date] + " as " + nameof(Date));
                    logger.Info("Read " + headings[EntryType] + " as " + nameof(EntryType));
                    logger.Info("Read " + headings[Book] + " as " + nameof(Book));
                    logger.Info("Read " + headings[LedgerName] + " as " + nameof(LedgerName));
                    logger.Info("Read " + headings[AdditionalInformation] + " as " + nameof(AdditionalInformation));
                    logger.Info("Read " + headings[Credit] + " as " + nameof(Credit));
                    logger.Info("Read " + headings[Debit] + " as " + nameof(Debit));

                    var journalStatements = reader.ReadAllLines(sheet, index, (r) =>
                    {
                        var credit = r.ReadDouble(Credit);
                        var debit = r.ReadDouble(Debit);
                        if (credit > 0.001 && debit > 0.001)
                        {
                            logger.Error("Both credit and debit has values");
                        }
                        return new JournalStatement
                        {
                            Date = r.ReadDate(Date),
                            EntryType = r.ReadString(EntryType),
                            BookName = r.ReadString(Book).GetBook(),
                            Name = r.ReadString(LedgerName),
                            AdditionalName = r.ReadString(AdditionalInformation),
                            Value = credit - debit,
                        };
                    }).ToList();
                    return journalStatements;
                }
            }
        }


        public BalanceSheetBook GetBalanceSheet(ILogger logger)
        {
            AssertSheetExists(_excelFileName, _sheetName);
            using (var package = ExcelSheetInfoProvider.GetReadOnlyExcelPackage(_excelFileName))
            {
                using (var sheet = package.Workbook.Worksheets[_sheetName])
                {
                    const int LedgerName = 0;
                    const int Credit = 1;
                    const int Debit = 2;

                    ExcelReader reader = new ExcelReader();
                    int index = 0;
                    string[] headings = reader.ReadLine(sheet, index++,
                        r => new string[]
                        {
                            r.ReadString(LedgerName),
                            r.ReadString(Credit),
                            r.ReadString(Debit),
                        });
                    logger.Info("Read " + headings[LedgerName] + " as " + nameof(LedgerName));
                    logger.Info("Read " + headings[Credit] + " as " + nameof(Credit));
                    logger.Info("Read " + headings[Debit] + " as " + nameof(Debit));

                    var balanceSheetStatements = reader.ReadAllLines(sheet, index, (r) =>
                    {
                        var credit = r.ReadDouble(Credit);
                        var debit = r.ReadDouble(Debit);
                        if (credit > 0.001 && debit > 0.001)
                        {
                            logger.Error("Both credit and debit has values");
                        }
                        return new Statement()
                        {
                            Name = r.ReadString(LedgerName),
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
}
