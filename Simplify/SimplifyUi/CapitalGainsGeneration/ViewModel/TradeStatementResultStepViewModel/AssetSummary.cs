using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
            Quantity = x.Quantity;
            PurchaseDate = x.PurchaseDate;
            PurchaseValue = x.PurchaseValue;
            PurchasePerUnit = x.GetCostPerUnit();

            if (x.IsSquared)
            {
                SaleDate = x.SaleDate;
                SaleValue = x.SaleValue;
                SalePerUnit = x.GetSalePerUnit();
                Profit = x.GetProfit();
            }
            else
            {
                SaleDate = null;
                SaleValue = null;
                SalePerUnit =null;
                Profit = null;
            }
        }

        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "#.###")]
        public double Quantity { get; set; }

        [DisplayName("Purchase Date")]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy")]
        public DateTime? PurchaseDate { get; set; }

        [DisplayName("Cost")]
        [DisplayFormat(DataFormatString = "N2")]
        public double PurchaseValue { get; set; }


        [DisplayFormat(DataFormatString = "N2")]
        [DisplayName("Cost per Unit")]
        public double PurchasePerUnit { get; set; }

        [DisplayFormat(DataFormatString = "dd/MM/yyyy")]
        [DisplayName("Sale Date")]
        public DateTime? SaleDate { get; set; }

        [DisplayFormat(DataFormatString = "N2")]
        [DisplayName("Sale Value")]
        public double? SaleValue { get; set; }

        [DisplayName("Sale per Unit")]
        [DisplayFormat(DataFormatString = "N2")]
        public double? SalePerUnit { get; set; }

        [DisplayFormat(DataFormatString = "N2")]
        public double? Profit { get; set; }
    }
}