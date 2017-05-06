using System;
using Simplify.ExcelDataGateway;

namespace SimplifyUi.ViewModel
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

                BalanceSheetGateway balanceSheetGateway = new BalanceSheetGateway(InputExcelFileName);
                var logger = new Logger();
                
                var balanceSheet = balanceSheetGateway.GetBalanceSheet(logger, SelectedSheet);
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