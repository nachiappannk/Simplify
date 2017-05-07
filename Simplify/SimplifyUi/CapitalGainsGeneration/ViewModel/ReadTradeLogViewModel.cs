using System;
using Simplify.Application;
using Simplify.ExcelDataGateway;
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

                TradeLogGateway tradeLogGateway = new TradeLogGateway(InputExcelFileName);
                var tradeLogs = tradeLogGateway.ReadTradeLog(logger, SelectedSheet);
                SquaredAndOpenTradeSeparator squaredAndOpenTradeSeparator = new SquaredAndOpenTradeSeparator();
                var result = squaredAndOpenTradeSeparator.Separate(tradeLogs);
                {
                    TradeLogGateway writeLogGateway = new TradeLogGateway("Test.xlsx");
                    writeLogGateway.WriteOpenPositions(result.OpenTradeStatements);
                }

                Bag.AddObject(CapitalGainWorkflowViewModel.TradeLogKey, tradeLogs);
                Bag.AddObject(CapitalGainWorkflowViewModel.TradeLogReadMessagesKey,logger.GetLogMessages());
                NextStepRequestAction.Invoke(CapitalGainWorkflowViewModel.DisplayMessage);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }
}