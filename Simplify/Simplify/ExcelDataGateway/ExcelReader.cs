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
    public class ExcelReader
    {
        public IList<string> GetSheetNames(string excelFileName)
        {
            AssertFileExists(excelFileName);
            AssertFileExtentionIsXlsx(excelFileName);
            
            using (ExcelPackage package = GetReadOnlyExcelPackage(excelFileName))
            {
                return GetSheetNames(package);
            }
        }

        private static IList<string> GetSheetNames(ExcelPackage package)
        {
            return package.Workbook.Worksheets.Select(s => s.Name).ToList();
        }

        private static ExcelPackage GetReadOnlyExcelPackage(string excelFileName)
        {
            var stream = GetFileStream(excelFileName);
            return new ExcelPackage(stream);
        }

        public List<JournalStatement> GetJournalStatements(string excelFileName, string sheetName, out List<string> readMessages)
        {
            AssertFileExists(excelFileName);
            AssertFileExtentionIsXlsx(excelFileName);
            using (ExcelPackage package = GetReadOnlyExcelPackage(excelFileName))
            {
                AssertSheetExists(sheetName, package);
                ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];
                const int SerialNumber = 1;
                const int Date = 2;
                const int EntryType = 3;
                const int Book = 4;
                const int LedgerName = 5;
                const int AdditionalInformation = 6;
                const int Credit = 7;
                const int Debit = 8;

                int index = 1;
                readMessages = new List<string>();
                List<JournalStatement> ret = new List<JournalStatement>();
                try
                {
                    AddColumnReadInfo(readMessages, worksheet, index, SerialNumber, nameof(SerialNumber));
                    AddColumnReadInfo(readMessages, worksheet, index, Date, nameof(Date));
                    AddColumnReadInfo(readMessages, worksheet, index, EntryType, nameof(EntryType));
                    AddColumnReadInfo(readMessages, worksheet, index, Book, nameof(Book));
                    AddColumnReadInfo(readMessages, worksheet, index, LedgerName, nameof(LedgerName));
                    AddColumnReadInfo(readMessages, worksheet, index, AdditionalInformation, nameof(AdditionalInformation));
                    AddColumnReadInfo(readMessages, worksheet, index, Credit, nameof(Credit));
                    AddColumnReadInfo(readMessages, worksheet, index, Debit, nameof(Debit));
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
                            AddErrorMessage(readMessages, "Credit and Debit mentioned in row number " + index);
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
                    AddInfoMessage(readMessages, "Read "+(index-1)+" number of entries");
                }
                return ret;
            }
        }


        public BalanceSheetBook GetBalanceSheet(string excelFileName, string sheetName, out List<string> readMessages)
        {
            AssertFileExists(excelFileName);
            AssertFileExtentionIsXlsx(excelFileName);
            using (ExcelPackage package = GetReadOnlyExcelPackage(excelFileName))
            {
                AssertSheetExists(sheetName, package);
                readMessages = new List<string>();
                BalanceSheetBook ret = new BalanceSheetBook();
                ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];
                const int LedgerName = 1;
                const int Credit = 2;
                const int Debit = 3;

                int index = 1;

                try
                {
                    AddColumnReadInfo(readMessages, worksheet, index, LedgerName, nameof(LedgerName));
                    AddColumnReadInfo(readMessages, worksheet, index, Credit, nameof(Credit));
                    AddColumnReadInfo(readMessages, worksheet, index, Debit, nameof(Debit));

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
                                AddErrorMessage(readMessages, "Credit and Debit mentioned in row number " + index);
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
                    AddInfoMessage(readMessages, "Read " + (index - 1) + " number of entries");
                }
                AddInfoMessage(readMessages, "Please verify that the previous year capital is "+ret.GetCapital());
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
        private void AddColumnReadInfo(IList<string> readMessages, ExcelWorksheet worksheet, int rowIndex, int columnIndex, string fieldName)
        {
            var columnName = GetText(worksheet, rowIndex, columnIndex);
            AddInfoMessage(readMessages, "Read " + columnName + " as " + fieldName);
        }

        private static void AssertSheetExists(string sheetName, ExcelPackage package)
        {
            var sheetNames = GetSheetNames(package);
            if (!sheetNames.Contains(sheetName)) throw new Exception(sheetName + ": sheet does not exist");
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


        
        private static void AssertFileExtentionIsXlsx(string excelFileName)
        {
            if (Path.GetExtension(excelFileName) != ".xlsx")
            {
                throw new Exception("The extention is not xlsx");
            }
        }

        private static void AssertFileExists(string excelFileName)
        {
            if (!File.Exists(excelFileName))
            {
                throw new Exception("File Does Not Exist");
            }
        }
    }
}
