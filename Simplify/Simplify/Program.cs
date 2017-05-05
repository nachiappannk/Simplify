using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Simplify.Books;
using Simplify.DataGateway;
using Simplify.ExcelDataGateway;

namespace Simplify
{
    public class Logger : ILogger
    {
        public void Info(string message)
        {
            
        }

        public void Error(string message)
        {
        }
    }

    class Program
    {

        static void Main(string[] args)
        {

            var s =  @"C:\Users\IC021285\Desktop\SprintFeedbackReportIT85Jan.xlsx";

            FileStream stream = File.Open(s, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            int i = 0;

            //string journalFile;
            //string previousPeriodFile;
            //DateTime startDate;
            //DateTime endDate;
            ////if (args.Length < 1)
            ////{
            //    journalFile = "Test.xlsx";
            ////}
            ////if (args.Length < 2)
            ////{
            //    previousPeriodFile = "PreviousPeriod.xlsx";
            ////}
            ////if (args.Length < 3)
            //{ 
            //    startDate = new DateTime(2016, 03, 31);
            //}
            ////if (args.Length < 4)
            //{
            //    endDate = new DateTime(2017,03,31);
            //}

            //IJournalReader journalReader = new JournalReader();
            //var journal = journalReader.GetJournal(journalFile);
            //var preBalanceSheet = journalReader.GetBalanceSheet(previousPeriodFile);





            //var booksGenerator = new BooksOfAccountGenerator();
            //var books = booksGenerator.Generate(journal, preBalanceSheet, endDate, startDate);
            //DateTime now = DateTime.Now;
            //string outputExcelFileName = "Output" + now.ToString("_yyyy_MM_dd_HH_mm_ss_fff");
            //IBooksOfAccountWriter booksOfAccountWriter = new BooksOfAccountWriter(outputExcelFileName);
            //booksOfAccountWriter.WriteBooksOfAccount(books);
        }
    }
}
