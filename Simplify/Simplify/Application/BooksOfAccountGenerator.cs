using System;
using System.Collections.Generic;
using Simplify.Books;
using Simplify.ExcelDataGateway;

namespace Simplify.Application
{
    public class BooksOfAccountGenerator
    {
        private readonly ILogger _logger;

        public BooksOfAccountGenerator(ILogger logger)
        {
            _logger = logger;
        }

        public ConsolidatedBook Generate(IList<JournalStatement> journalStatements, BalanceSheetBook previousYearBalanceSheet,
            DateTime bookClosingDate, DateTime bookOpeningDate)
        {

            var trialBalance = new TrialBalanceGenerator(_logger).Generate(journalStatements);
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
                //ProfitAndLoss = profitAndLoss,
                //CapitalAccount = capitalAccount,
                //BalanceSheetBook = balanceSheet,
            };
        }
    }
}