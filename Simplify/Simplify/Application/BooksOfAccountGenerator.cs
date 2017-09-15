using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Simplify.Books;
using Simplify.ExcelDataGateway;

namespace Simplify.Application
{
    public class BooksOfAccountGenerator
    {
        private readonly ILogger _logger;
        public static JournalStatementBracketTrimmer _trimmer = new JournalStatementBracketTrimmer();

        public BooksOfAccountGenerator(ILogger logger)
        {
            _logger = logger;
        }

        private DetailedDatedStatement TrimBracket(DetailedDatedStatement detailedDatedStatement)
        {
            var retVal = detailedDatedStatement.CreateCopy();
            retVal.Description = _trimmer.Trim(detailedDatedStatement.Description);
            return retVal; 
        }

        private bool IsCapitalAccount(string name)
        {
            return name.EndsWith("(e)") || name.EndsWith("(E)");
        }

        private Statement TrimBracket(Statement detailedDatedStatement)
        {
            var ret = new Statement()
            {
                Description = _trimmer.Trim(detailedDatedStatement.Description),
                Value = detailedDatedStatement.Value
            };
            return ret;
        }

        public ConsolidatedBook Generate(IList<DetailedDatedStatement> journalStatements, BalanceSheetBook previousYearBalanceSheet,
            DateTime bookClosingDate, DateTime bookOpeningDate)
        {
            var balanceAccountNames = previousYearBalanceSheet.Select(x => x.Description).Distinct().ToList();
            var captialAccountNames = balanceAccountNames.Where(IsCapitalAccount).Select(_trimmer.Trim).ToList();
            

            var trialBalance = new TrialBalanceGenerator(_logger).Generate(journalStatements);

            var bracketTrimmedJournalStatements = journalStatements.Select(TrimBracket).ToList();

            var notionalBooksGenerator = new NotionalBooksGenerator();
            var realBooksGenerator = new RealBooksGenerator();

            foreach (var statement in bracketTrimmedJournalStatements)
            {
                if (notionalBooksGenerator.IsNotionalAccountStatement(statement))
                    notionalBooksGenerator.AddNotionalAccountStatement(statement);
                else
                    realBooksGenerator.AddStatement(statement, StatementPriority.Normal);
            }

            var notionalAccountBooks = notionalBooksGenerator.GetNotionalAccountBooks();

            var notionalAccountClosingStatements = GetClosingStatements(notionalAccountBooks, bookClosingDate);
            realBooksGenerator.AddStatements(notionalAccountClosingStatements, StatementPriority.PreClosing);

            var previousBalanceSheetStatements = GetOpeningStatements(previousYearBalanceSheet.Select(TrimBracket).ToList(), bookOpeningDate);
            realBooksGenerator.AddStatements(previousBalanceSheetStatements, StatementPriority.Opening);

            var realBooks = realBooksGenerator.GetRealAccountBooks(captialAccountNames);
            var balanceSheetBook = realBooksGenerator.GetBalanceSheetBook();
            foreach (var balanceSheetStatement in balanceSheetBook)
            {
                if (captialAccountNames.Contains(balanceSheetStatement.Description))
                    balanceSheetStatement.Description = balanceSheetStatement.Description + " (E)";
            }

            return new ConsolidatedBook()
            {
                Journal    = journalStatements,
                TrialBalance = trialBalance,
                NotionalAccountBooks = notionalAccountBooks,
                RealAccountBooks = realBooks,
                BalanceSheetBook = balanceSheetBook,
            };
        }

        private static List<DetailedDatedStatement> GetOpeningStatements(List<Statement> previousYearBalanceSheet, DateTime bookOpeningDate)
        {
            var previousBalanceSheetStatements = previousYearBalanceSheet.Select(x => new DetailedDatedStatement()
            {
                Date = bookOpeningDate,
                Value = x.Value,
                Description = x.Description,
                DetailedDescription = "Opening Balance",
            }).ToList();
            return previousBalanceSheetStatements;
        }

        private static List<DetailedDatedStatement> GetClosingStatements(List<NotionalAccountBook> notionalAccountBooks, DateTime bookClosingDate)
        {
            var notionalAccountClosingStatements = notionalAccountBooks.Select(x => new DetailedDatedStatement()
            {
                Date = bookClosingDate,
                Value = x.GetTotal(),
                Description = x.Account.RealAccountName,
                DetailedDescription = $"Closing of {x.Account.NotionalAccountName}",
            }).ToList();
            return notionalAccountClosingStatements;
        }
    }
}