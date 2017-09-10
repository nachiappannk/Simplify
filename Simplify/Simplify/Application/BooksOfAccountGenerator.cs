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
        private readonly StatementPriorityAdjuster _statementPriorityAdjuster;

        public BooksOfAccountGenerator(ILogger logger)
        {
            _logger = logger;
            _statementPriorityAdjuster = new StatementPriorityAdjuster();
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

            bracketTrimmedJournalStatements = bracketTrimmedJournalStatements.Select(_statementPriorityAdjuster.ReDetailedDatedStatementAsNormal).ToList();

            var notionalBooksGenerator = new NotionalBooksGenerator();

            var nonNotionalStatements = new List<DetailedDatedStatement>();

            foreach (var statement in bracketTrimmedJournalStatements)
            {
                if (notionalBooksGenerator.IsNotionalAccountStatement(statement))
                    notionalBooksGenerator.AddNotionalAccountStatement(statement);
                else
                    nonNotionalStatements.Add(statement);
            }

            var notionalAccountBooks = notionalBooksGenerator.GetNotionalAccountBooks();



            //var profitAndLoss = new ProfitAndLossAccountGenerator().Generate(journalStatements);

            //var capitalAccount = new CapitalAccountGenerator().Generate(journalStatements,
            //    previousYearBalanceSheet.GetCapital(),
            //    profitAndLoss.GetNetEarnings(),
            //    bookOpeningDate, bookClosingDate);

            //var balanceSheet = new BalanceSheetGenerator().Generate(journalStatements, previousYearBalanceSheet,
            //    capitalAccount.GetCapital());

            return new ConsolidatedBook()
            {
                Journal    = journalStatements,
                TrialBalance = trialBalance,
                NotionalAccountBooks = notionalAccountBooks,
                //ProfitAndLoss = profitAndLoss,
                //CapitalAccount = capitalAccount,
                //BalanceSheetBook = balanceSheet,
            };
        }
    }
}