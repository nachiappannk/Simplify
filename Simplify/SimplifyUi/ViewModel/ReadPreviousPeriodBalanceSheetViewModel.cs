using System;
using System.Collections.Generic;
using Simplify.ExcelDataGateway;

namespace Simplify.ViewModel
{
    public class ReadPreviousPeriodBalanceSheetViewModel : ReadExcelViewModel
    {
        public ReadPreviousPeriodBalanceSheetViewModel(Bag bag, Action<int> nextStepRequestAction) 
            : base(bag, nextStepRequestAction)
        {
            ReadFileName = "Balance Sheet Excel File";
            ReadSheetName = "Previous year Balance Sheet work sheet";
            Title = "Please provide the previous year balance sheet";
        }

        protected override void ExecuteNextStep()
        {
            try
            {
                ComputeAndUpdateSheetName();

                BooksOfAccountReader booksOfAccountReader = new BooksOfAccountReader(InputExcelFileName, SelectedSheet);
                var logger = new Logger();
                
                var balanceSheet = booksOfAccountReader.GetBalanceSheet(logger);
                _bag.AddObject(ConsolidatedBooksGenerationWorkflowViewModel.InputBalanceSheetKey, balanceSheet);
                _bag.AddObject(ConsolidatedBooksGenerationWorkflowViewModel.BalanceSheetReadMessagesKey, logger.GetLogMessages());
                _nextStepRequestAction.Invoke(ConsolidatedBooksGenerationWorkflowViewModel.DisplayBalanceSheetReadMessages);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }
}