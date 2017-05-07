using System;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel
{
    public class ReadTradeLogViewModel : ReadExcelViewModel
    {
        public ReadTradeLogViewModel(Bag bag, Action<int> nextStepRequestAction)
            : base(bag, nextStepRequestAction)
        {
            ReadFileNameDisplayText = "Trade log Excel File";
            ReadSheetNameDisplayText = "Trade log Sheet work sheet";
            TitleDisplayText = "Please provide the trade log";
        }

        protected override void ExecuteNextStep()
        {
            try
            {
                ComputeSheetName();

                var logger = new Logger();

                //BalanceSheetGateway balanceSheetGateway = new BalanceSheetGateway(InputExcelFileName);
                //var balanceSheet = balanceSheetGateway.GetBalanceSheet(logger, SelectedSheet);
                //Bag.AddObject(BooksOfAccountGenerationWorkflowViewModel.InputBalanceSheetKey, balanceSheet);
                //Bag.AddObject(BooksOfAccountGenerationWorkflowViewModel.BalanceSheetReadMessagesKey, logger.GetLogMessages());
                NextStepRequestAction.Invoke(CapitalGainWorkflowViewModel.DisplayMessage);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }
}