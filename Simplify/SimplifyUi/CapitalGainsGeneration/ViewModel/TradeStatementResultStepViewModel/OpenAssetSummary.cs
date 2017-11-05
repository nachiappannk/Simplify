using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Trade;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class OpenAssetSummary : AssetSummary
    {
        protected readonly Dictionary<string, List<SquarableStatement>> _dictionary;
        private readonly ProcessedTradeStatementsContainer _container;

        public OpenAssetSummary(ProcessedTradeStatementsContainer container)
        {
            _container = container;
            _dictionary = container.OpenAssetSummaryBooks;
            AssetNames = _dictionary.Keys.ToList();


        }

        public List<AssetEvaluationSummarizedRecord> SummaryRecords { get; set; }

        private List<AssetSummaryRecord> _records;
        public List<AssetSummaryRecord> Records
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

        protected override void OnAssetSelectedChanged(string selectedAsset)
        {
            var openAssetSummaryBook = _dictionary[selectedAsset];
            Records = openAssetSummaryBook.Select(x => new AssetSummaryRecord(x)).ToList();
            
                SummaryRecords = _container.PurchasedAssetSummarizedStatements
                    .Where(x => x.Name == SelectedAsset)
                    .Select(x => new AssetEvaluationSummarizedRecord(x))
                    .ToList();
            FirePropertyChanged(nameof(SummaryRecords));
        }
    }
}