using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Trade;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class OpenAssetSummary : AssetSummary<OpenAssetSummaryBook>
    {
        private readonly ProcessedTradeStatementsContainer _container;

        public OpenAssetSummary(ProcessedTradeStatementsContainer container) : base(container.OpenAssetSummaryBooks)
        {
            _container = container;
            OnAssetSelectedChanged(SelectedAsset);
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
            Records = openAssetSummaryBook.Statements.Select(x => new AssetSummaryRecord(x)).ToList();
            if (_container != null)
            {
                SummaryRecords = _container.PurchasedAssetSummarizedStatements
                    .Where(x => x.Name == SelectedAsset)
                    .Select(x => new AssetEvaluationSummarizedRecord(x))
                    .ToList();
            }
            
            FirePropertyChanged(nameof(SummaryRecords));
        }
    }
}