using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using OfficeOpenXml;
using Simplify.Books;
using Simplify.DataGateway;

namespace Simplify.ExcelDataGateway
{
    public class BooksOfAccountWriter : IBooksOfAccountWriter
    {
        private string outputExcelFileName;

        public BooksOfAccountWriter(string outputExcelFileName)
        {

            this.outputExcelFileName = outputExcelFileName;
        }

        public void WriteBooksOfAccount(ConsolidatedBook consolidatedBook)
        {
            FileInfo newFile = new FileInfo(outputExcelFileName);
            if (newFile.Exists)
            {
                newFile.Delete(); // ensures we create a new workbook
                newFile = new FileInfo(outputExcelFileName);
            }
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                AddJournal(package, consolidatedBook.Journal);
                AddTrialBalance(package, consolidatedBook.TrialBalance);
                AddProfitAndLoss(package, consolidatedBook.ProfitAndLoss);
                AddCapitalAccount(package, consolidatedBook.CapitalAccount);
                AddBalanceSheet(package, consolidatedBook.BalanceSheetBook);
            }

            
        }

        private void AddBalanceSheet(ExcelPackage package, BalanceSheetBook balanceSheet1)
        {
            ExcelWorksheet worksheet = AddWorksheet(package, "BS");
            var bSheetCopy = balanceSheet1.Where(s => Math.Abs(s.Value) > 0.001).ToList();


            var index = 1;
            AddTableHeading(worksheet, index++, "Ledger", "Credit", "Debit");
            AddStatements(bSheetCopy, worksheet, index);
            index = index + bSheetCopy.Count;
            AddTableClosing(bSheetCopy, worksheet, index, "Total");
            package.Save();
        }


        private void AddDate(ExcelWorksheet sheet, int row, int column, DateTime date)
        {
            sheet.Cells[row, column].Value = date;
            sheet.Cells[row, column].Style.Numberformat.Format = "dd-mmm-yyyy";
        }

        private void Add2DecimalNumber(ExcelWorksheet sheet, int row, int column, double value)
        {
            sheet.Cells[row, column].Value = value;
            sheet.Cells[row, column].Style.Numberformat.Format = "#,##0.00";
        }

        private void Add0DecimalNumber(ExcelWorksheet sheet, int row, int column, double value)
        {
            sheet.Cells[row, column].Value = value;
        }

        private void AddText(ExcelWorksheet sheet, int row, int column, string value)
        {
            sheet.Cells[row, column].Value = value;
        }

        private void AddCapitalAccount(ExcelPackage package, CapitalAccountBook capitalAccountBook)
        {
            ExcelWorksheet worksheet = AddWorksheet(package, "CapitalAccount");

            int index = 1;

            AddTableHeading(worksheet, index++, "Date","Ledger","Credit","Debit");
            AddDateStatements(capitalAccountBook, worksheet, index);
            index = index + capitalAccountBook.Count;
            AddDatedTableClosing(capitalAccountBook, worksheet, index, "Closing Balance");
            package.Save();
        }

        private void AddDatedTableClosing(CapitalAccountBook capitalAccountBook, ExcelWorksheet worksheet, int index,
            string totalName)
        {
            AddDate(worksheet, index, 1, capitalAccountBook.ClosingDate);
            AddText(worksheet, index, 2, totalName);
            Add2DecimalNumber(worksheet, index, 3, capitalAccountBook.GetCreditTotal());
            Add2DecimalNumber(worksheet, index, 4, capitalAccountBook.GetDebitTotal());
            Add2DecimalNumber(worksheet, index, 5, capitalAccountBook.GetTotal());
        }

        private void AddDateStatements(IList<DatedStatement> datedStatements, ExcelWorksheet worksheet, int startingRow)
        {
            foreach (var statement in datedStatements)
            {
                AddDate(worksheet, startingRow, 1, statement.Date);
                AddText(worksheet, startingRow, 2, statement.Name);
                Add2DecimalNumber(worksheet, startingRow, 3, statement.GetCreditValue());
                Add2DecimalNumber(worksheet, startingRow, 4, statement.GetDebitValue());
                startingRow++;
            }
        }

        private static ExcelWorksheet AddWorksheet(ExcelPackage package, string sheetName)
        {
            return package.Workbook.Worksheets.Add(sheetName);
        }

        private void AddTableHeading(ExcelWorksheet worksheet, int startingRow, params string[] headings)
        {
            var index = 1;
            foreach (var heading in headings)
            {
                AddText(worksheet, startingRow, index++, heading);
            }
        }


        private void AddProfitAndLoss(ExcelPackage package, ProfitAndLossBook profitAndLoss)
        {
            ExcelWorksheet worksheet = AddWorksheet(package,"P&L");

            int index = 1;
            AddTableHeading(worksheet, index++, "Ledger", "Earnings", "Expenditure");
            AddStatements(profitAndLoss, worksheet, index);
            index = profitAndLoss.Count + index;
            AddTableClosing(profitAndLoss, worksheet, index, "Net Earnings");
            package.Save();
        }

        private void AddTableClosing(IList<Statement> statements, ExcelWorksheet worksheet, int index, string totalName)
        {
            AddText(worksheet, index, 1, totalName);
            Add2DecimalNumber(worksheet, index, 2, statements.GetCreditTotal());
            Add2DecimalNumber(worksheet, index, 3, statements.GetDebitTotal());
            Add2DecimalNumber(worksheet, index, 4, statements.GetTotal());
        }

        private void AddStatements(IList<Statement> statements, ExcelWorksheet worksheet, int startingRow)
        {
            foreach (var statement in statements)
            {
                AddText(worksheet, startingRow, 1, statement.Name);
                Add2DecimalNumber(worksheet, startingRow, 2, statement.GetCreditValue());
                Add2DecimalNumber(worksheet, startingRow, 3, statement.GetDebitValue());
                startingRow++;
            }
        }


        private void AddTrialBalance(ExcelPackage package, TrialBalanceBook trialBalance )
        {
            ExcelWorksheet worksheet = AddWorksheet(package,"TRB");

            var index = 1;
            AddTableHeading(worksheet, index++, "Ledger", "Credit", "Debit");
            AddStatements(trialBalance, worksheet, index);
            index = index + trialBalance.Count;
            AddTableClosing(trialBalance, worksheet, index,"Total");
            package.Save();
        }

        private void AddJournal(ExcelPackage package, IList<JournalStatement> journalStatements)
        {
            var index = 1;
            ExcelWorksheet worksheet = AddWorksheet(package,"Journal");
            AddTableHeading(worksheet, index++, "S.No.", "Date", "Type", "Book", "Ledger", "Description", "Credit", "Debit");
            
            foreach (var statement in journalStatements)
            {
                Add0DecimalNumber(worksheet, index, 1, index - 1);
                AddDate(worksheet, index, 2, statement.Date);
                AddText(worksheet, index, 3, statement.EntryType);
                AddText(worksheet, index, 4, statement.BookName.GetString());
                AddText(worksheet, index, 5, statement.Name);
                AddText(worksheet, index, 6, statement.AdditionalName);
                Add2DecimalNumber(worksheet, index, 7, statement.GetCreditValue());
                Add2DecimalNumber(worksheet, index, 8, statement.GetDebitValue());
                index++;
            }
            package.Save();
        }
    }
}