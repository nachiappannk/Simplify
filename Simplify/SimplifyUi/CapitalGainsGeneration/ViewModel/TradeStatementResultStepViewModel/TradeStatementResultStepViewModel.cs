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

        private AssetNamesViewModel _assetNamesViewModel;
        public AssetNamesViewModel AssetNamesViewModel
        {
            get { return _assetNamesViewModel; }
            set
            {
                if (_assetNamesViewModel != value)
                {
                    _assetNamesViewModel = value;
                    FirePropertyChanged();
                }
            }
        }

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

        private CostBookViewModel _constBookViewModel;

        public CostBookViewModel ConstBookViewModel
        {
            get { return _constBookViewModel; }
            set
            {
                if (_constBookViewModel != value)
                {
                    _constBookViewModel = value;
                    FirePropertyChanged();
                }
            }
        }

        public AssetSummary AssetSummary { get; set; }

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
            AssetNamesViewModel = new AssetNamesViewModel(statementsContainer);
            OpenPositionViewModel = new OpenPositionViewModel(_statementsContainer);
            ProfitBookViewModel = new ProfitBookViewModel(_statementsContainer);
            AssetSummary = new AssetSummary(_statementsContainer.AssetSummaryBooks);
            ConstBookViewModel = new CostBookViewModel(_statementsContainer);

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