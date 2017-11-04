using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Simplify.ExcelDataGateway.Trade;
using Simplify.Trade;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Properties;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class TradeStatementResultStepViewModel : WorkFlowStepViewModel
    {
        private ProcessedTradeStatementsContainer _statementsContainer;

        public InteractionRequest<FileSaveAsNotification> FileSaveAsRequest { get; private set; }
        public AssetEvaluationViewModel AssetEvaluationViewModel { get; set; }
        public ProfitBookViewModel ProfitBookViewModel { get; set; }
        public AssetQuotesViewModel AssetQuotesViewModel { get; set; }
        public AssetEvaluationSummarizedViewModel AssetEvaluationSummarizedViewModel { get; set; }
        public OpenAssetSummary HoldingAssetSummary { get; set; }
        public ClosedAssetSummary ClosedAssetSummary { get; set; }

        public TradeStatementResultStepViewModel()
        {
            CanGoToNext = false;
            CanGoToHome = true;
            CanGoToPrevious = true;
            Name = "Result";
            FileSaveAsRequest = new InteractionRequest<FileSaveAsNotification>();
            var saveCommand = new NamedCommand("Save", new DelegateCommand(SaveOutputFile));
            AddCommand(saveCommand);

        }
        public void SetStatements(ProcessedTradeStatementsContainer statementsContainer)
        {
            _statementsContainer = statementsContainer;
            AssetQuotesViewModel = new AssetQuotesViewModel(statementsContainer);

            AssetEvaluationViewModel = new AssetEvaluationViewModel(_statementsContainer);

            AssetEvaluationSummarizedViewModel = new AssetEvaluationSummarizedViewModel(_statementsContainer);

            
            ProfitBookViewModel = new ProfitBookViewModel(_statementsContainer);
            HoldingAssetSummary = new OpenAssetSummary(_statementsContainer);
            ClosedAssetSummary = new ClosedAssetSummary(_statementsContainer.ClosedAssetSummaryBooks);
            
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