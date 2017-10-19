using System.IO;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Simplify.Application;
using Simplify.ExcelDataGateway;
using SimplifyUi.Common.ViewModel;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel
{
    public class TradeStatementResultStepViewModel : WorkFlowStepViewModel
    {
        public InteractionRequest<FileSaveAsNotification> FileSaveAsRequest { get; private set; }
        private ProcessedTradeStatementsContainer _statementsContainer;

        private NamedCommand SaveCommand;

        public TradeStatementResultStepViewModel()
        {
            FileSaveAsRequest = new InteractionRequest<FileSaveAsNotification>();
            SaveCommand = new NamedCommand("Save", new DelegateCommand(SaveOutputFile));
            AddCommand(SaveCommand);

        }
        public void SetStatements(ProcessedTradeStatementsContainer statementsContainer)
        {
            _statementsContainer = statementsContainer;
        }

        
        public void SaveOutputFile()
        {
            var file = new FileSaveAsNotification()
            {
                Title = "Capital Gains Output File",
                DefaultFileName = "CapitalGainsOutput",
            };
            FileSaveAsRequest.Raise(file);
            var fullPath = file.OutputFileName;

            var writer = new ProcessedTradeStatementsExcelGateway();
            if (File.Exists(fullPath)) File.Delete(fullPath);
            writer.Write(fullPath, _statementsContainer);
        }
    }
}