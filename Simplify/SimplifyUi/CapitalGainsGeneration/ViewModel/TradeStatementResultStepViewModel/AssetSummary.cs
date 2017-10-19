using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Simplify.Trade;
using SimplifyUi.Common;
using SimplifyUi.Properties;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
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
                    var records = _dictionary[_selectedAsset];
                    Records = records.Select(x => new AssetSummaryRecord(x)).ToList();
                }
            }
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
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void FirePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }

    public class AssetSummaryRecord
    {
        public AssetSummaryRecord(SquarableStatement x)
        {
            Name = x.Name;
            Quantity = x.Quantity.ToStringWithNumberOfDecimals(2);
            PurchaseDate = x.PurchaseDate.ToStringDisplayable();
            PurchaseValue = x.PurchaseValue.ToStringWithNumberOfDecimals(2);
            SaleDate = x.IsSquared? x.SaleDate.ToStringDisplayable(): String.Empty;
            SaleValue = x.IsSquared ? x.SaleValue.ToStringWithNumberOfDecimals(2) : String.Empty;
            Profit = x.IsSquared ? x.GetProfit().ToStringWithNumberOfDecimals(2) : String.Empty;
        }

        public string Name { get; set; }
        public string Quantity { get; set; }

        [DisplayName("Purchase Date")]
        public string PurchaseDate { get; set; }

        [DisplayName("Cost")]
        public string PurchaseValue { get; set; }

        [DisplayName("Sale Date")]
        public string SaleDate { get; set; }

        [DisplayName("Sale Value")]
        public string SaleValue { get; set; }
        public string Profit { get; set; }
    }
}