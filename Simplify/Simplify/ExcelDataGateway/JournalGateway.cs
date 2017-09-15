using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Application;
using Simplify.Books;

namespace Simplify.ExcelDataGateway
{
    public class JournalGateway
    {
        private readonly string _inputFile;

        private List<List<string>> _headingOptions = new List<List<string>>()
        {
            new List<string>() { "S.No."},
            new List<string>() { "Date" },
            new List<string>() { "Ledger" },
            new List<string>() { "DetailedDescription" },
            new List<string>() { "Credit" },
            new List<string>() { "Debit" }
        };

        const int SerialNumber = 0;
        const int Date = 1;
        const int LedgerName = 2;
        const int Description = 3;
        const int Credit = 4;
        const int Debit = 5;

        public JournalGateway(string inputFile)
        {
            _inputFile = inputFile;
        }

        public void WriteJournal(IList<DetailedDatedStatement> journalStatements)
        {
            using (ExcelWriter writer = new ExcelWriter(_inputFile, "Journal"))
            {
                int index = 0;

                object[] headings = new object[_headingOptions.Count];
                for(int i = 0; i < _headingOptions.Count; i++)
                {
                    var head = _headingOptions[i].FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(head)) head = string.Empty;
                    headings[i] = head;    
                }
                writer.Write(index++, headings);
                writer.SetColumnsWidth(6, 12, 35, 45, 12, 12);
                writer.ApplyHeadingFormat(6);
                writer.WriteList(index, journalStatements.OrderBy(x =>x.Date).ToList(),
                    (j, rowIndex) => new object[]
                    {
                        rowIndex - 1,
                        j.Date,
                        j.Description,
                        j.DetailedDescription,
                        j.GetCreditValue(),
                        j.GetDebitValue(),
                    });

            }
        }


        public List<DetailedDatedStatement> GetJournalStatements(ILogger logger, string sheetName)
        {
            
            using (ExcelReader reader = new ExcelReader(_inputFile, sheetName, logger))
            {

                SheetHeadingVerifier.VerifyHeadingNames(logger, reader, _headingOptions);

                var journalStatements = reader.ReadAllLines(1, (r) =>
                {
                    var isCreditAvailable = r.IsValueAvailable(Credit);
                    var credit = isCreditAvailable ? r.ReadDouble(Credit) : 0;
                    var isDebitAvailable = r.IsValueAvailable(Debit);
                    var debit = isDebitAvailable ? r.ReadDouble(Debit) : 0;
                    if (isCreditAvailable && isDebitAvailable)
                    {
                        logger.Log(MessageType.IgnorableError, $"In file{r.FileName}, " +
                                                               $"in sheet{r.SheetName}, " +
                                                               $"in line no. {r.LineNumber}, " +
                                                               "both credit and debit is mentioned. Taking the difference as value");
                    }
                    if (!isCreditAvailable && !isDebitAvailable)
                    {
                        logger.Log(MessageType.IgnorableError, $"In file{r.FileName}, " +
                                                               $"in sheet{r.SheetName}, " +
                                                               $"in line no. {r.LineNumber}, " +
                                                               "both credit and debit is not mentioned. Taking the value as 0");
                    }
                    return new DetailedDatedStatement
                    {
                        Date = r.ReadDate(Date),
                        Description = r.ReadString(LedgerName),
                        DetailedDescription = r.ReadString(Description),
                        Value = credit - debit,
                    };
                }).ToList();
                return journalStatements;
            }
        }

    }
}