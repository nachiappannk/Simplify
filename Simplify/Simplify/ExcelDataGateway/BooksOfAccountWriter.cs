using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Simplify.Books;
using Simplify.DataGateway;
using System.Drawing;

namespace Simplify.ExcelDataGateway
{
    public class ExcelWriter : IDisposable
    {
        private ExcelPackage _package;
        private ExcelWorksheet _workSheet;
        public ExcelWriter(string fileName, string sheetName)
        {
            FileInfo file = new FileInfo(fileName);
            _package = new ExcelPackage(file);
            _workSheet = _package.Workbook.Worksheets.Add(sheetName);

        }

        public void Write(int zeroBasedRowIndex, params object[] values)
        {
            for (int j = 0; j < values.Length; j++)
            {
                int column = j + 1;
                int row = zeroBasedRowIndex + 1;
                _workSheet.Cells[row, column].Value = values[j];
                if (values[j] is double)
                    _workSheet.Cells[row, column].Style.Numberformat.Format = "#,##0.00";
                else if (values[j] is DateTime)
                    _workSheet.Cells[row, column].Style.Numberformat.Format = "dd-mmm-yy";
            }
        }

        public void WriteList<T>(int zeroBasedStartingRowIndex, IList<T> items,
                Func<T ,int, object[]> itemInterpretor)
        {
            int row = zeroBasedStartingRowIndex;
            foreach (var item in items)
            {
                Write(row++, itemInterpretor.Invoke(item, row));
            }
        }

        public void SetColumnsWidth(params double[] widths)
        {
            for (int i = 1; i <= widths.Length; i++)
            {
                _workSheet.Column(i).Width = widths[i - 1];
            }
        }

        public void ApplyHeadingFormat(int numberOfColumns)
        {
            using (var range = _workSheet.Cells[1, 1, 1, numberOfColumns])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                range.Style.Font.Color.SetColor(Color.White);
                range.AutoFilter = true;
            }
        }

        public void Dispose()
        {
            _package.Save();
            _package.Dispose();
        }
    }



    public class BooksOfAccountWriter : IBooksOfAccountWriter
    {
        private string outputExcelFileName;

        public BooksOfAccountWriter(string outputExcelFileName)
        {
            this.outputExcelFileName = outputExcelFileName;
        }

        public void WriteBooksOfAccount(ConsolidatedBook consolidatedBook)
        {
            AddJournal(consolidatedBook.Journal);
            AddTrialBalance(consolidatedBook.TrialBalance);
            AddProfitAndLoss(consolidatedBook.ProfitAndLoss);
            AddCapitalAccount(consolidatedBook.CapitalAccount);
            AddBalanceSheet(consolidatedBook.BalanceSheetBook);
        }

        private void AddBalanceSheet(BalanceSheetBook balanceSheet)
        {
            var bSheetCopy = balanceSheet.Where(s => Math.Abs(s.Value) > 0.001).ToList();
            var index = 0;
            using (var writer = new ExcelWriter(outputExcelFileName, "BS"))
            {
                object[] headings = {"S.No.", "Ledger", "Credit", "Debit", "Total"};
                writer.Write(index++,headings);
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

        private void AddCapitalAccount(CapitalAccountBook capitalAccountBook)
        {
            using (ExcelWriter writer = new ExcelWriter(outputExcelFileName, "CapitalAccount"))
            {
                int index = 0;
                object[] headings = {"S.No.", "Date", "Ledger", "Credit", "Debit", "Total"};
                writer.Write(index++, headings);
                writer.ApplyHeadingFormat(headings.Length);
                writer.SetColumnsWidth(6, 12, 45, 12, 12, 12);

                writer.WriteList(index, capitalAccountBook,
                    (j, rowIndex) => new object[]
                    {
                        rowIndex - 1,
                        j.Date,
                        j.Name,
                        j.GetCreditValue(),
                        j.GetDebitValue(),
                    });
                index = index + capitalAccountBook.Count;
                writer.Write(index, "", "Closing Balance", capitalAccountBook.GetCreditTotal(), 
                    capitalAccountBook.GetDebitTotal(),
                    capitalAccountBook.GetTotal());
                
            }
        }
        private void AddProfitAndLoss(ProfitAndLossBook profitAndLoss)
        {
            using (ExcelWriter writer = new ExcelWriter(outputExcelFileName, "P&L"))
            {
                int index = 0;
                writer.Write(index++, "S.No.", "Ledger", "Earnings", "Expenditure", "Net");
                writer.ApplyHeadingFormat(5);
                writer.SetColumnsWidth(6, 45, 12, 12, 12);

                writer.WriteList(index, profitAndLoss,
                    (j, rowIndex) => new object[]
                    {
                        rowIndex - 1,
                        j.Name,
                        j.GetCreditValue(),
                        j.GetDebitValue(),
                    });
                index = index + profitAndLoss.Count;
                writer.Write(index,"","NetEarnings", profitAndLoss.GetCreditTotal(), profitAndLoss.GetDebitTotal(),
                    profitAndLoss.GetTotal());
            }
        }

        private void AddTrialBalance(TrialBalanceBook trialBalance )
        {
            using (ExcelWriter writer = new ExcelWriter(outputExcelFileName, "TRB"))
            {
                int index = 0;
                writer.Write(index++, "S.No.", "Ledger", "Credit", "Debit","Difference");
                writer.SetColumnsWidth(6, 45, 12, 12, 12);
                writer.ApplyHeadingFormat(5);
                writer.WriteList(index, trialBalance,
                    (j, rowIndex) => new object[]
                    {
                        rowIndex- 1,
                        j.Name,
                        j.GetCreditValue(),
                        j.GetDebitValue(),
                    });
                index = index + trialBalance.Count;
                writer.Write(index,"", "Total", trialBalance.GetCreditTotal(), trialBalance.GetDebitTotal(), 
                    trialBalance.GetTotal());
            }
        }

        private void AddJournal(IList<JournalStatement> journalStatements)
        {
            using (ExcelWriter writer = new ExcelWriter(outputExcelFileName, "Journal"))
            {
                int index = 0;
                writer.Write(index++, "S.No.", "Date", "Type", "Book", "Ledger", "Description", "Credit", "Debit");
                writer.SetColumnsWidth(6, 12, 4, 8, 35, 45, 12, 12);
                writer.ApplyHeadingFormat(8);
                writer.WriteList(index, journalStatements, 
                    (j, rowIndex) => new object[]
                    {
                        rowIndex - 1,
                        j.Date,
                        j.EntryType,
                        j.BookName.GetString(),
                        j.Name,
                        j.AdditionalName,
                        j.GetCreditValue(),
                        j.GetDebitValue(),
                    });
                
            }
        }
    }
}