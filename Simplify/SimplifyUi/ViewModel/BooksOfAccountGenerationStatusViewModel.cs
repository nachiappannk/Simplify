using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Prism.Commands;
using Simplify.Books;
using Simplify.DataGateway;
using Simplify.ExcelDataGateway;

namespace Simplify.ViewModel
{
    public class BooksOfAccountGenerationStatusViewModel
    {
        public BooksOfAccountGenerationStatusViewModel(Bag bag)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string outputExcelFileName = "Output" + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss_fff");
                string fullPath = path + "\\" + outputExcelFileName + ".xlsx";


                var journal = bag.GetObject<List<JournalStatement>>(ConsolidatedBooksGenerationWorkflowViewModel.InputJournalKey);
                var previousBalanceSheet =
                    bag.GetObject<BalanceSheetBook>(ConsolidatedBooksGenerationWorkflowViewModel.InputBalanceSheetKey);

                BooksOfAccountGenerator booksOfAccountGenerator = new BooksOfAccountGenerator();
                var books = booksOfAccountGenerator.Generate(journal, previousBalanceSheet, new DateTime(2017, 3, 31),
                    new DateTime(2016, 3, 31));

                IBooksOfAccountWriter booksOfAccountWriter = new BooksOfAccountWriter(fullPath);
                booksOfAccountWriter.WriteBooksOfAccount(books);
                CompletedMessage = "Please find " + outputExcelFileName+".xlsx in the desktop";
                IsCompleted = true;
                IsError = false;
            }
            catch (Exception e)
            {
                IsError = true;
                IsCompleted = false;
                ErrorMessage = "Could not generate the file due to "+e.Message;
            }
            

        }

        public bool IsCompleted { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string CompletedMessage { get; set; }

    }
}