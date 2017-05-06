using System.Collections.Generic;
using System.Linq;
using Simplify.Books;

namespace Simplify.ExcelDataGateway
{
    public class JournalGateway
    {
        private readonly string _inputFile;
        string[] _headings = { "S.No.", "Date", "Type", "Book", "Ledger", "Description", "Credit", "Debit" };


        const int SerialNumber = 0;
        const int Date = 1;
        const int EntryType = 2;
        const int Book = 3;
        const int LedgerName = 4;
        const int Description = 5;
        const int Credit = 6;
        const int Debit = 7;

        public JournalGateway(string inputFile)
        {
            _inputFile = inputFile;
        }

        public void WriteJournal(IList<JournalStatement> journalStatements)
        {
            using (ExcelWriter writer = new ExcelWriter(_inputFile, "Journal"))
            {
                int index = 0;

                
                writer.Write(index++, _headings);
                writer.SetColumnsWidth(6, 12, 4, 8, 35, 45, 12, 12);
                writer.ApplyHeadingFormat(8);
                writer.WriteList(index, journalStatements.OrderBy(x =>x.Date).ToList(),
                    (j, rowIndex) => new object[]
                    {
                        rowIndex - 1,
                        j.Date,
                        j.EntryType,
                        j.BookName.GetString(),
                        j.Name,
                        j.Description,
                        j.GetCreditValue(),
                        j.GetDebitValue(),
                    });

            }
        }


        public List<JournalStatement> GetJournalStatements(ILogger logger, string sheetName)
        {
            
            using (ExcelReader reader = new ExcelReader(_inputFile, sheetName))
            {

                SheetHeadingLogger.LogHeadingRowDetails(logger, reader, _headings);

                var journalStatements = reader.ReadAllLines(1, (r) =>
                {
                    var credit = r.ReadDouble(Credit);
                    var debit = r.ReadDouble(Debit);
                    if (credit > 0.001 && debit > 0.001)
                    {
                        logger.Error("Both credit and debit has values");
                    }
                    return new JournalStatement
                    {
                        Date = r.ReadDate(Date),
                        EntryType = r.ReadString(EntryType),
                        BookName = r.ReadString(Book).GetBook(),
                        Name = r.ReadString(LedgerName),
                        Description = r.ReadString(Description),
                        Value = credit - debit,
                    };
                }).ToList();
                return journalStatements;
            }
        }

    }
}