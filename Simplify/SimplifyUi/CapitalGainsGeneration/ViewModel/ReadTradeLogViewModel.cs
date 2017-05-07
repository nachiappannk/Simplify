using System;
using Simplify.Application;
using Simplify.ExcelDataGateway;
using Simplify.Facade;
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
                CapitalGainsStatementGenerationFacade facade = new CapitalGainsStatementGenerationFacade();
                facade.GenerateStatements(InputExcelFileName,SelectedSheet,"Text.xlsx", logger);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }
}