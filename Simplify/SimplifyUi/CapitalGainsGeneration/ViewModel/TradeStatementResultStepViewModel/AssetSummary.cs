using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Simplify.Trade;
using SimplifyUi.Common;
using SimplifyUi.Common.ViewModelTools;
using SimplifyUi.Properties;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public abstract class AssetSummary<T> : NotifiesPropertyChanged
    {
        protected readonly Dictionary<string, T> _dictionary;

        protected AssetSummary(Dictionary<string,T> dictionary)
        {
            _dictionary = dictionary;
            AssetNames = dictionary.Keys.ToList();
            if (AssetNames.Count > 0)
            {
                SelectedAsset = AssetNames.ElementAt(0);
                IsEnabled = true;
            }
            else
            {
                IsEnabled = false;
            }
        }

        public bool IsEnabled { get; set; }
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
                    OnAssetSelectedChanged(_selectedAsset);
                }
            }
        }

        protected abstract void OnAssetSelectedChanged(string selectedAsset);
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

        [DisplayFormat(DataFormatString = CommonDefinition.QuantityDisplayFormat)]
        public double Quantity { get; set; }

        [DisplayName("Purchase Date")]
        [DisplayFormat(DataFormatString = CommonDefinition.DateDisplayFormat)]
        public DateTime? PurchaseDate { get; set; }

        [DisplayName("Cost")]
        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        public double PurchaseValue { get; set; }


        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Cost per Unit")]
        public double PurchasePerUnit { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.DateDisplayFormat)]
        [DisplayName("Sale Date")]
        public DateTime? SaleDate { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Sale Value")]
        public double? SaleValue { get; set; }

        [DisplayName("Sale per Unit")]
        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        public double? SalePerUnit { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        public double? Profit { get; set; }
    }
}