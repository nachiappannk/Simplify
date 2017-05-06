using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplify.Books;
using Simplify.DataGateway;

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OfficeOpenXml;

namespace Simplify.ExcelDataGateway
{
    class JournalReader : IJournalReader
    {
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
        

        public IList<JournalStatement> GetJournal(string inputExcelFile)
        {
            FileInfo inputFileInfo = new FileInfo(inputExcelFile);
            var ret = new List<JournalStatement>();
            if (!inputFileInfo.Exists)
            {
                return ret;
            }

            using (ExcelPackage package = new ExcelPackage(inputFileInfo, true))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                try
                {
                    int index = 2;
                    while (true)
                    {
                        var ledgerName = GetText(worksheet, index, 5);
                        var additionalName = GetText(worksheet, index, 6);
                        var credit = GetNumber(worksheet, index, 7);
                        var debit = GetNumber(worksheet, index, 8);
                        var value = credit - debit;
                        var date = GetDate(worksheet, index, 2);
                        var book = GetBook(worksheet, index, 4);
                        var entryType = GetText(worksheet, index, 3);
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
                    
                }
                return ret;
            }
        }

        public BalanceSheetBook GetBalanceSheet(string previousPeriodFile)
        {
            FileInfo inputFileInfo = new FileInfo(previousPeriodFile);
            var ret = new BalanceSheetBook();
            if (!inputFileInfo.Exists)
            {
                return ret;
            }
            using (ExcelPackage package = new ExcelPackage(inputFileInfo, true))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                try
                {
                    int index = 2;
                    while (true)
                    {
                        var ledgerName = GetText(worksheet, index, 1);
                        var credit = GetNumber(worksheet, index, 2);
                        var debit = GetNumber(worksheet, index, 3);
                        var value = credit - debit;
                        if (ledgerName.ToLower() != "total")
                        {
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

                }
                return ret;
            }
        }
    }
}
