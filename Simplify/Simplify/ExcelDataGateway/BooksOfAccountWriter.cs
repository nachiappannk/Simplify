using System.Collections.Generic;
using System.Runtime.InteropServices;
using Simplify.Books;
using Simplify.DataGateway;

namespace Simplify.ExcelDataGateway
{
    public class BooksOfAccountWriter : IBooksOfAccountWriter
    {
        private readonly string outputExcelFileName;

        public BooksOfAccountWriter(string outputExcelFileName)
        {
            this.outputExcelFileName = outputExcelFileName;
        }

        public void WriteBooksOfAccount(ConsolidatedBook consolidatedBook)
        {
            AddJournal(consolidatedBook.Journal);
            AddTrialBalance(consolidatedBook.TrialBalance);
            //AddProfitAndLoss(consolidatedBook.ProfitAndLoss);
            //AddCapitalAccount(consolidatedBook.CapitalAccount);
            //AddBalanceSheet(consolidatedBook.BalanceSheetBook);
        }

        private void AddBalanceSheet(BalanceSheetBook balanceSheet)
        {
            BalanceSheetGateway balanceSheetGateway = new BalanceSheetGateway(outputExcelFileName);
            balanceSheetGateway.WriteBalanceSheet(balanceSheet);
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
                writer.Write(index, "","", "Closing Balance", capitalAccountBook.GetCreditTotal(), 
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
            JournalGateway journalGateway = new JournalGateway(outputExcelFileName);
            journalGateway.WriteJournal(journalStatements);   
        }
    }
}