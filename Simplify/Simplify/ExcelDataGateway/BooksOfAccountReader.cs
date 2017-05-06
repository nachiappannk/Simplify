using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Simplify.Books;

namespace Simplify.ExcelDataGateway
{
    public interface IRowReader
    {
        DateTime ReadDate(int zeroBasedColumnIndex);
        int ReadInteger(int zeroBasedColumnIndex);
        double ReadDouble(int zeroBasedColumnIndex);
        string ReadString(int zeroBasedColumnIndex);
    }


    public class BooksOfAccountReader : IDisposable
    {
        private ExcelPackage _excelPackage;
        private ExcelWorksheet _worksheet;

        public BooksOfAccountReader(string excelFileName, string sheetName)
        {
            ExcelSheetInfoProvider sheetInfoProvider = new ExcelSheetInfoProvider(excelFileName);
            if (!sheetInfoProvider.IsSheetPresent(sheetName))
            {
                throw new Exception(sheetName + ": sheet does not exist");
            }
            _excelPackage = ExcelSheetInfoProvider.GetReadOnlyExcelPackage(excelFileName);
            _worksheet = _excelPackage.Workbook.Worksheets[sheetName];
        }

        public List<JournalStatement> GetJournalStatements(ILogger logger)
        {

            ExcelWorksheet worksheet = _worksheet;
                const int SerialNumber = 1;
                const int Date = 2;
                const int EntryType = 3;
                const int Book = 4;
                const int LedgerName = 5;
                const int AdditionalInformation = 6;
                const int Credit = 7;
                const int Debit = 8;

                int index = 1;
                List<JournalStatement> ret = new List<JournalStatement>();
                try
                {
                    AddColumnReadInfo(logger, worksheet, index, SerialNumber, nameof(SerialNumber));
                    AddColumnReadInfo(logger, worksheet, index, Date, nameof(Date));
                    AddColumnReadInfo(logger, worksheet, index, EntryType, nameof(EntryType));
                    AddColumnReadInfo(logger, worksheet, index, Book, nameof(Book));
                    AddColumnReadInfo(logger, worksheet, index, LedgerName, nameof(LedgerName));
                    AddColumnReadInfo(logger, worksheet, index, AdditionalInformation, nameof(AdditionalInformation));
                    AddColumnReadInfo(logger, worksheet, index, Credit, nameof(Credit));
                    AddColumnReadInfo(logger, worksheet, index, Debit, nameof(Debit));
                    index++;
                    while (true)
                    {
                        var serialNumber = GetText(worksheet, index, SerialNumber);
                        var date = GetDate(worksheet, index, Date);
                        var entryType = GetText(worksheet, index, EntryType);
                        var book = GetBook(worksheet, index, Book);
                        var ledgerName = GetText(worksheet, index, LedgerName);
                        var additionalName = GetText(worksheet, index, AdditionalInformation);
                        var credit = GetNumber(worksheet, index, Credit);
                        var debit = GetNumber(worksheet, index, Debit);
                        if (credit > 0.001 && debit > 0.001)
                        {
                            logger.Error("Credit and Debit mentioned in row number " + index);
                        }
                        var value = credit - debit;   
                        ret.Add(new JournalStatement()
                        {
                            Name = ledgerName,
                            Value = value,
                            Date = date,
                            BookName = book,
                            AdditionalName = additionalName,
                            EntryType = entryType,
                        });
                        index++;
                    }
                }
                catch (Exception e)
                {
                    logger.Info("Read "+(index-1)+" number of entries");
                }
                return ret;
        }


        public BalanceSheetBook GetBalanceSheet(ILogger logger)
        {
            
            {
                BalanceSheetBook ret = new BalanceSheetBook();
                ExcelWorksheet worksheet = _worksheet;
                const int LedgerName = 1;
                const int Credit = 2;
                const int Debit = 3;

                int index = 1;

                try
                {
                    AddColumnReadInfo(logger, worksheet, index, LedgerName, nameof(LedgerName));
                    AddColumnReadInfo(logger, worksheet, index, Credit, nameof(Credit));
                    AddColumnReadInfo(logger, worksheet, index, Debit, nameof(Debit));

                    index = 2;
                    while (true)
                    {
                        
                        var ledgerName = GetText(worksheet,index, 1);
                        var credit = GetNumber(worksheet, index, 2);
                        var debit = GetNumber(worksheet, index, 3);
                        var value = credit - debit;
                        if (ledgerName.ToLower() != "total")
                        {
                            if (credit > 0.001 && debit > 0.001)
                            {
                                logger.Error("Credit and Debit mentioned in row number " + index);
                            }
                            ret.Add(new Statement()
                            {
                                Name = ledgerName,
                                Value = value,
                            });
                        }
                        index++;
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Read " + (index - 1) + " number of entries");
                }
                logger.Error("Please verify that the previous year capital is " +ret.GetCapital());
                return ret;
            }
        }

        private static void AddErrorMessage(IList<string> readMessages, string errorMessage)
        {
            readMessages.Add("Error: "+errorMessage);
        }

        private static void AddInfoMessage(IList<string> readMessages, string infoMessage)
        {
            readMessages.Add("Info: " + infoMessage);
        }
        private void AddColumnReadInfo(ILogger logger, ExcelWorksheet worksheet, int rowIndex, int columnIndex, string fieldName)
        {
            var columnName = GetText(worksheet, rowIndex, columnIndex);
            logger.Info("Read " + columnName + " as " + fieldName);
        }

        

        private static FileStream GetFileStream(string excelFileName)
        {
            FileStream stream = File.Open(excelFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return stream;
        }

        private string GetText(ExcelWorksheet sheet, int row, int column)
        {
            return sheet.Cells[row, column].Value.ToString();
        }

        private DateTime GetDate(ExcelWorksheet sheet, int row, int column)
        {
            var dateValue = (double)sheet.Cells[row, column].Value;
            return DateTime.FromOADate(dateValue);
        }

        private double GetNumber(ExcelWorksheet sheet, int row, int column)
        {
            return Convert.ToDouble(sheet.Cells[row, column].Value);
        }

        private Book GetBook(ExcelWorksheet sheet, int row, int column)
        {
            var bookName = sheet.Cells[row, column].Value.ToString();
            return bookName.ToUpper().GetBook();
        }

        public void Dispose()
        {
            _excelPackage.Dispose();
        }
    }
}
