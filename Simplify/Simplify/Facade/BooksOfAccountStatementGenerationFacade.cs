using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplify.Application;
using Simplify.Books;
using Simplify.DataGateway;
using Simplify.ExcelDataGateway;

namespace Simplify.Facade
{
    public class BooksOfAccountStatementGenerationFacade
    {
        public void GenerateStatements(string journalExcel,
            string journalSheet, string previousBalanceSheetExcel, string previousBalanceSheetWorksheet, 
            string outputExcelFile, DateTime startDate, DateTime endDate, ILogger logger)
        {
            if (File.Exists(outputExcelFile)) File.Delete(outputExcelFile);

            JournalGateway journalGateway = new JournalGateway(journalExcel);
            var journal = journalGateway.GetJournalStatements(logger, journalSheet);

            BalanceSheetGateway balanceSheetGateway = new BalanceSheetGateway(previousBalanceSheetExcel);
            var previousBalanceSheet = balanceSheetGateway.GetBalanceSheet(logger, previousBalanceSheetWorksheet);
            

            BooksOfAccountGenerator booksOfAccountGenerator = new BooksOfAccountGenerator(logger);
            var books = booksOfAccountGenerator.Generate(journal, previousBalanceSheet, endDate,
               startDate);

            IBooksOfAccountWriter booksOfAccountWriter = new BooksOfAccountWriter(outputExcelFile);
            booksOfAccountWriter.WriteBooksOfAccount(books);
        }
    }
}
