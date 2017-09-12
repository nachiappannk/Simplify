using System;
using System.Collections.Generic;
using System.Linq;
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
            return _trimmer.Trim(detailedDatedStatement);
        }

        public ConsolidatedBook Generate(IList<DetailedDatedStatement> journalStatements, BalanceSheetBook previousYearBalanceSheet,
            DateTime bookClosingDate, DateTime bookOpeningDate)
        {
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

            var previousBalanceSheetStatements = GetOpeningStatements(previousYearBalanceSheet, bookOpeningDate);
            realBooksGenerator.AddStatements(previousBalanceSheetStatements, StatementPriority.Opening);

            var realBooks = realBooksGenerator.GetRealAccountBooks();
            var balanceSheetBook = realBooksGenerator.GetBalanceSheetBook();

            return new ConsolidatedBook()
            {
                Journal    = journalStatements,
                TrialBalance = trialBalance,
                NotionalAccountBooks = notionalAccountBooks,
                RealAccountBooks = realBooks,
                BalanceSheetBook = balanceSheetBook,
                //ProfitAndLoss = profitAndLoss,
                //CapitalAccount = capitalAccount,
                //BalanceSheetBook = balanceSheet,
            };
        }

        private static List<DetailedDatedStatement> GetOpeningStatements(BalanceSheetBook previousYearBalanceSheet, DateTime bookOpeningDate)
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
                DetailedDescription = $"Closing of {x.Account.RealAccountName}",
            }).ToList();
            return notionalAccountClosingStatements;
        }
    }
}