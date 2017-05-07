using System;
using Simplify.ExcelDataGateway;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.BooksOfAccountGeneration.ViewModel
{
    public class ReadPreviousPeriodBalanceSheetViewModel : ReadExcelViewModel
    {
        public ReadPreviousPeriodBalanceSheetViewModel(Bag bag, Action<int> nextStepRequestAction) 
            : base(bag, nextStepRequestAction)
        {
            ReadFileNameDisplayText = "Balance Sheet Excel File";
            ReadSheetNameDisplayText = "Previous year Balance Sheet work sheet";
            TitleDisplayText = "Please provide the previous year balance sheet";
        }

        protected override void ExecuteNextStep()
        {
            try
            {
                ComputeSheetName();

                BalanceSheetGateway balanceSheetGateway = new BalanceSheetGateway(InputExcelFileName);
                var logger = new Logger();
                
                var balanceSheet = balanceSheetGateway.GetBalanceSheet(logger, SelectedSheet);
                Bag.AddObject(BooksOfAccountGenerationWorkflowViewModel.InputBalanceSheetKey, balanceSheet);
                Bag.AddObject(BooksOfAccountGenerationWorkflowViewModel.BalanceSheetReadMessagesKey, logger.GetLogMessages());
                NextStepRequestAction.Invoke(BooksOfAccountGenerationWorkflowViewModel.DisplayBalanceSheetReadMessages);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }
}