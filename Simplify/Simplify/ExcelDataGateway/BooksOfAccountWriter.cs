using System.Collections.Generic;
using System.Runtime.InteropServices;
using Simplify.Books;
using Simplify.DataGateway;

namespace Simplify.ExcelDataGateway
{
    public class BooksOfAccountWriter : IBooksOfAccountWriter
    {
        private readonly string _outputExcelFileName;

        public BooksOfAccountWriter(string outputExcelFileName)
        {
            _outputExcelFileName = outputExcelFileName;
        }

        public void WriteBooksOfAccount(ConsolidatedBook consolidatedBook)
        {
            AddJournal(consolidatedBook.Journal);
            AddTrialBalance(consolidatedBook.TrialBalance);
            AddNotionalAccountBooks(consolidatedBook.NotionalAccountBooks);
            AddReadAccounts(consolidatedBook.RealAccountBooks);
            AddBalanceSheet(consolidatedBook.BalanceSheetBook);

            //AddProfitAndLoss(consolidatedBook.ProfitAndLoss);
            //AddCapitalAccount(consolidatedBook.CapitalAccount);
        }

        private void AddBalanceSheet(BalanceSheetBook balanceSheet)
        {
            BalanceSheetGateway balanceSheetGateway = new BalanceSheetGateway(_outputExcelFileName);
            balanceSheetGateway.WriteBalanceSheet(balanceSheet);
        }

        private void AddCapitalAccount(CapitalAccountBook capitalAccountBook)
        {
            using (ExcelWriter writer = new ExcelWriter(_outputExcelFileName, "CapitalAccount"))
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
                        j.Description,
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
            using (ExcelWriter writer = new ExcelWriter(_outputExcelFileName, "P&L"))
            {
                int index = 0;
                writer.Write(index++, "S.No.", "Ledger", "Earnings", "Expenditure", "Net");
                writer.ApplyHeadingFormat(5);
                writer.SetColumnsWidth(6, 45, 12, 12, 12);

                writer.WriteList(index, profitAndLoss,
                    (j, rowIndex) => new object[]
                    {
                        rowIndex - 1,
                        j.Description,
                        j.GetCreditValue(),
                        j.GetDebitValue(),
                    });
                index = index + profitAndLoss.Count;
                writer.Write(index,"","NetEarnings", profitAndLoss.GetCreditTotal(), profitAndLoss.GetDebitTotal(),
                    profitAndLoss.GetTotal());
            }
        }


        private void AddNotionalAccountBooks(List<NotionalAccountBook> notionalAccountBooks)
        {
            foreach (var notionalAccountBook in notionalAccountBooks)
            {
                WriteNotionalAccount(notionalAccountBook);
                WriteNotionalAccountSummary(notionalAccountBook);
            }
        }

        private void AddReadAccounts(List<RealAccountBook> realAccountBooks)
        {
            foreach (var realAccountBook in realAccountBooks)
            {
                using (ExcelWriter writer =
                    new ExcelWriter(_outputExcelFileName, realAccountBook.AccountName))
                {
                    int index = 0;
                    writer.Write(index++, "S.No.", "Date", "Description", "Credit", "Debit", "Net");
                    writer.ApplyHeadingFormat(6);
                    writer.SetColumnsWidth(6, 12, 45, 12, 12, 12);
                    writer.WriteList(index, realAccountBook, (j, rowIndex) =>
                        new object[]
                        {
                            rowIndex - 1,
                            j.Date,
                            j.Description,
                            j.GetCreditValue(),
                            j.GetDebitValue(),
                        });
                    index = index + realAccountBook.Count + 1;
                    writer.Write(index, "", "", "Net " + realAccountBook.AccountName,
                        realAccountBook.GetCreditTotal(), realAccountBook.GetDebitTotal(),
                        realAccountBook.GetTotal());
                }
            }
        }

        private void WriteNotionalAccountSummary(NotionalAccountBook notionalAccountBook)
        {
            using (ExcelWriter writer =
                new ExcelWriter(_outputExcelFileName, notionalAccountBook.Account.NotionalAccountName + "-Summary"))
            {
                int index = 0;
                writer.Write(index++, "S.No.", "Description", "Credit", "Debit", "Net");
                writer.ApplyHeadingFormat(5);
                writer.SetColumnsWidth(6, 45, 12, 12, 12);

                var summaryStatement = notionalAccountBook.GetSummaryStatement();
                writer.WriteList(index, summaryStatement,
                    (j, rowIndex) => new object[]
                    {
                        rowIndex - 1,
                        j.Description,
                        j.GetCreditValue(),
                        j.GetDebitValue(),
                    });
                index = index + summaryStatement.Count + 1;
                writer.Write(index, "", "Net " + notionalAccountBook.Account.NotionalAccountName,
                    summaryStatement.GetCreditTotal(), summaryStatement.GetDebitTotal(),
                    summaryStatement.GetTotal());
            }
        }

        private void WriteNotionalAccount(NotionalAccountBook notionalAccountBook)
        {
            using (ExcelWriter writer = new ExcelWriter(_outputExcelFileName, notionalAccountBook.Account.NotionalAccountName))
            {
                int index = 0;
                writer.Write(index++, "S.No.", "Date", "Tag", "Description", "Credit", "Debit", "Net");
                writer.ApplyHeadingFormat(7);
                writer.SetColumnsWidth(6, 12, 30, 45, 12, 12, 12);

                writer.WriteList(index, notionalAccountBook,
                    (j, rowIndex) => new object[]
                    {
                        rowIndex - 1,
                        j.Date,
                        j.Description,
                        j.DetailedDescription,
                        j.GetCreditValue(),
                        j.GetDebitValue(),
                    });
                index = index + notionalAccountBook.Count + 1;
                writer.Write(index, "", "", "", "Net " + notionalAccountBook.Account.NotionalAccountName,
                    notionalAccountBook.GetCreditTotal(), notionalAccountBook.GetDebitTotal(),
                    notionalAccountBook.GetTotal());
            }


        }

        private void AddTrialBalance(TrialBalanceBook trialBalance )
        {
            using (ExcelWriter writer = new ExcelWriter(_outputExcelFileName, "TRB"))
            {
                int index = 0;
                writer.Write(index++, "S.No.", "Ledger", "Credit", "Debit","Difference");
                writer.SetColumnsWidth(6, 45, 12, 12, 12);
                writer.ApplyHeadingFormat(5);
                writer.WriteList(index, trialBalance,
                    (j, rowIndex) => new object[]
                    {
                        rowIndex- 1,
                        j.Description,
                        j.GetCreditValue(),
                        j.GetDebitValue(),
                    });
                index = index + 1 +trialBalance.Count;
                writer.Write(index,"", "Total", trialBalance.GetCreditTotal(), trialBalance.GetDebitTotal(), 
                    trialBalance.GetTotal());
            }
        }

        private void AddJournal(IList<DetailedDatedStatement> journalStatements)
        {
            JournalGateway journalGateway = new JournalGateway(_outputExcelFileName);
            journalGateway.WriteJournal(journalStatements);   
        }
    }
}