using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Trade;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class OpenAssetSummary : AssetSummary<OpenAssetSummaryBook>
    {
        public OpenAssetSummary(Dictionary<string, OpenAssetSummaryBook> dictionary) : base(dictionary)
        {
        }

        protected override void OnAssetSelectedChanged(string selectedAsset)
        {
            var openAssetSummaryBook = _dictionary[selectedAsset];
            Records = openAssetSummaryBook.Select(x => new AssetSummaryRecord(x)).ToList();
            OpenQuantity = openAssetSummaryBook.QuanityOfOpenPosition;
            AverageCost = openAssetSummaryBook.AverageCost;
            OpenPositionCost = openAssetSummaryBook.OpenPositionCost;
        }


        private double _openQuantity;

        public double OpenQuantity
        {
            get { return _openQuantity; }
            set
            {
                if (Math.Abs(_openQuantity - value) > 0.001)
                {
                    _openQuantity = value;
                    FirePropertyChanged();
                }
            }
        }

        private double _averageCost;
        public double AverageCost
        {
            get { return _averageCost; }
            set
            {
                if (Math.Abs(_averageCost - value) > 0.001)
                {
                    _averageCost = value;
                    FirePropertyChanged();
                }
            }
        }

        private double _openPositionCost;
        public double OpenPositionCost
        {
            get { return _openPositionCost; }
            set
            {
                if (Math.Abs(_openPositionCost - value) > 0.001)
                {
                    _openPositionCost = value;
                    FirePropertyChanged();
                }
            }
        }
    }
}