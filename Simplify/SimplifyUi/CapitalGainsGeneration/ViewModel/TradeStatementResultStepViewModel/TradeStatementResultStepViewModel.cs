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
    public class TradeStatementResultStepViewModel : WorkFlowStepViewModel, INotifyPropertyChanged
    {
        public InteractionRequest<FileSaveAsNotification> FileSaveAsRequest { get; private set; }
        private ProcessedTradeStatementsContainer _statementsContainer;

        

        private OpenPositionViewModel _openPositionViewModel;
        public OpenPositionViewModel OpenPositionViewModel
        {
            get { return _openPositionViewModel; }
            set
            {
                if (_openPositionViewModel != value)
                {
                    _openPositionViewModel = value;
                    FirePropertyChanged();
                }
            } 
        }

        private ProfitBookViewModel _profitBookViewModel;

        public ProfitBookViewModel ProfitBookViewModel
        {
            get { return _profitBookViewModel; }
            set
            {
                if (_profitBookViewModel != value)
                {
                    _profitBookViewModel = value;
                    FirePropertyChanged();
                }
            }
        }
        
        private AssetEvaluationBookViewModel _assetEvaluationBookViewModel;
        public AssetEvaluationBookViewModel AssetEvaluationBookViewModel
        {
            get{ return _assetEvaluationBookViewModel;}
            set
            {
                if (value != _assetEvaluationBookViewModel)
                {
                    _assetEvaluationBookViewModel = value;
                    FirePropertyChanged();
                }
            }
        }

        public AssetQuotesViewModel AssetQuotesViewModel { get; set; }
        public AssetEvaluationAggregatedBookViewModel AssetEvaluationAggregatedBookViewModel { get; set; }

        public OpenAssetSummary HoldingAssetSummary { get; set; }

        public ClosedAssetSummary ClosedAssetSummary { get; set; }

        private NamedCommand SaveCommand;

        public TradeStatementResultStepViewModel()
        {
            CanGoToNext = false;
            CanGoToHome = true;
            CanGoToPrevious = true;
            Name = "Result";
            FileSaveAsRequest = new InteractionRequest<FileSaveAsNotification>();
            SaveCommand = new NamedCommand("Save", new DelegateCommand(SaveOutputFile));
            AddCommand(SaveCommand);

        }
        public void SetStatements(ProcessedTradeStatementsContainer statementsContainer)
        {
            _statementsContainer = statementsContainer;
            OpenPositionViewModel = new OpenPositionViewModel(_statementsContainer);
            ProfitBookViewModel = new ProfitBookViewModel(_statementsContainer);
            HoldingAssetSummary = new OpenAssetSummary(_statementsContainer);
            ClosedAssetSummary = new ClosedAssetSummary(_statementsContainer.ClosedAssetSummaryBooks);
            AssetEvaluationBookViewModel = new AssetEvaluationBookViewModel(_statementsContainer.AssetEvalutionBook);
            AssetEvaluationAggregatedBookViewModel = new AssetEvaluationAggregatedBookViewModel(_statementsContainer);
            AssetQuotesViewModel = new AssetQuotesViewModel(statementsContainer);
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void FirePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}