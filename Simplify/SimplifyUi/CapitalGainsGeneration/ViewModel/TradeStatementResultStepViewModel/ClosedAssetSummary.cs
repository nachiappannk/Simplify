using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Trade;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class ClosedAssetSummary : AssetSummary<ClosedAssetSummaryBook>
    {
        public ClosedAssetSummary(Dictionary<string, ClosedAssetSummaryBook> dictionary) : base(dictionary)
        {
        }


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
            var closedAssetSummaryBook = _dictionary[selectedAsset];
            Records = closedAssetSummaryBook.Select(x => new AssetSummaryRecord(x)).ToList();
            Profit = closedAssetSummaryBook.Profit;
        }

        private double _profit;
        public double Profit
        {
            get { return _profit; }
            set
            {
                if (Math.Abs(_profit - value) > 0.001)
                {
                    _profit = value;
                    FirePropertyChanged();
                }
            }
        }
    }
}