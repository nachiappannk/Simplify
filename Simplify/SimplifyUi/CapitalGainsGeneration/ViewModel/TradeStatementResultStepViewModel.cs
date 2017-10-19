using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Simplify.Application;
using Simplify.Books;
using Simplify.ExcelDataGateway;
using SimplifyUi.Annotations;
using SimplifyUi.Common.ViewModel;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel
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

        private List<OpenPositionTableRecord> _openPositions;
        public List<OpenPositionTableRecord> OpenPositions
        {
            get { return _openPositions; }
            set
            {
                if (_openPositions != value)
                {
                    _openPositions = value;
                    FirePropertyChanged();
                }
            } 
        }

        private List<ProfitBookTableRecord> _profitBook;

        public List<ProfitBookTableRecord> ProfitBook
        {
            get { return _profitBook; }
            set
            {
                if (_profitBook != value)
                {
                    _profitBook = value;
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
            AssetNamesViewModel = new AssetNamesViewModel();
            AssetNamesViewModel.AssetNames = new List<AssetNameTableRecord>();
            AssetNamesViewModel.AssetNames.AddRange(_statementsContainer.AssetNamesBook.Select(x => new AssetNameTableRecord {AssetName = x}));
            OpenPositions = new List<OpenPositionTableRecord>();
            OpenPositions.AddRange(_statementsContainer.OpenPositionBook.Select(x => new OpenPositionTableRecord
            {
                SerialNumber = x.SerialNumber,
                Date = x.Date,
                Name = x.Name,
                Quantity = x.Quantity,
                Value = x.Value,
                TransactionTax = x.TransactionTax,
                TransactionDetail = x.TransactionDetail,
            }));
            ProfitBook = new List<ProfitBookTableRecord>();
            ProfitBook.AddRange(_statementsContainer.ProfitBook.Select(x => new ProfitBookTableRecord()
            {
                Name = x.Name,
                Quantity = x.Quantity,
                PurchaseDate = x.PurchaseDate,
                PurchaseValue = x.PurchaseValue,
                PurchaseTransactionTax = x.PurchaseTransactionTax,
                PurchaseTransactionDetail = x.PurchaseTransactionDetail,
                SaleDate = x.SaleDate,
                SaleValue = x.SaleValue,
                SaleTransactionDetail = x.SaleTransactionDetail,
                SaleTransactionTax = x.SaleTransactionTax,
            }));
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

    public class AssetNameTableRecord
    {
        public string AssetName { get; set; }
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

    


    public class AssetNamesViewModel
    {
        public List<AssetNameTableRecord> AssetNames { get; set; }
    }

    public class OpenPositionTableRecord
    {
        public int SerialNumber { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }
        public string TransactionTax { get; set; }
        public string TransactionDetail { get; set; }
    }

    public class ProfitBookTableRecord
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public double PurchaseValue { get; set; }
        public string PurchaseTransactionTax { get; set; }
        public string PurchaseTransactionDetail { get; set; }
        public DateTime SaleDate { get; set; }
        public double SaleValue { get; set; }
        public string SaleTransactionTax { get; set; }
        public string SaleTransactionDetail { get; set; }
    }

}