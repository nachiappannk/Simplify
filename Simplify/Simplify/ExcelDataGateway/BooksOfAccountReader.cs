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

        public List<JournalStatement> GetJournalStatements(ILogger logger)
        {

            const int SerialNumber = 0;
            const int Date = 1;
            const int EntryType = 2;
            const int Book = 3;
            const int LedgerName = 4;
            const int AdditionalInformation = 5;
            const int Credit = 6;
            const int Debit = 7;

            string[] columnNames =
            {
                nameof(SerialNumber),
                nameof(Date),
                nameof(EntryType),
                nameof(Book),
                nameof(LedgerName),
                nameof(AdditionalInformation),
                nameof(Credit),
                nameof(Debit)
            };

            using (ExcelReader reader = new ExcelReader(_excelFileName, _sheetName))
            {

                SheetHeadingLogger.LogHeadingRowDetails(logger, reader, columnNames);

                var journalStatements = reader.ReadAllLines(1, (r) =>
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


        public BalanceSheetBook GetBalanceSheet(ILogger logger)
        {
            const int LedgerName = 0;
            const int Credit = 1;
            const int Debit = 2;
            string[] columnNames = { nameof(LedgerName), nameof(Credit), nameof(Debit)};

            using (ExcelReader reader = new ExcelReader(_excelFileName, _sheetName))
            { 
                SheetHeadingLogger.LogHeadingRowDetails(logger, reader, columnNames);
            
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
