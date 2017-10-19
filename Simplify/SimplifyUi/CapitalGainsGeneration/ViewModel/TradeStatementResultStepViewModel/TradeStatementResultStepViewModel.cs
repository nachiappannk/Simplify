using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

        private List<CostStatement> _effectiveCostBook;

        public List<CostStatement> EffectiveCostBook
        {
            get { return _effectiveCostBook; }
            set
            {
                if (_effectiveCostBook != value)
                {
                    _effectiveCostBook = value;
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
            EffectiveCostBook = new List<CostStatement>();
            EffectiveCostBook.AddRange(_statementsContainer.EffectiveCostStatementBook.Select(x => x));
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

    

    public class AssetSummary : INotifyPropertyChanged
    {
        private readonly Dictionary<string, List<SquarableStatement>> _dictionary;

        public AssetSummary(Dictionary<string,List<SquarableStatement>> dictionary)
        {
            _dictionary = dictionary;
            AssetNames = dictionary.Keys.ToList();
            SelectedAsset = AssetNames.ElementAt(0);

        }
        public List<string> AssetNames { get; set; }

        private string _selectedAsset;

        public string SelectedAsset
        {
            get { return _selectedAsset; }
            set
            {
                if (_selectedAsset != value)
                {
                    _selectedAsset = value;
                    Records = _dictionary[_selectedAsset];
                }
            }
        }

        private List<SquarableStatement> _records;
        public List<SquarableStatement> Records
        {
            get { return _records; }
            set
            {
                if (_records != value)
                {
                    _records = value;
                    FirePropertyChanged();
                }
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void FirePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }


    
}