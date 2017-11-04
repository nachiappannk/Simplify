using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Simplify.CommonDefinitions;
using Simplify.Trade;
using SimplifyUi.Annotations;
using SimplifyUi.Common.ViewModelTools;
using System.ComponentModel.DataAnnotations;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class AssetEvaluationSummarizedViewModel
    {
        public AssetEvaluationSummarizedViewModel(ProcessedTradeStatementsContainer container)
        {
            Records = container.PurchasedAssetSummarizedStatements.Select(x => new AssetEvaluationSummarizedRecord(x)).ToList();
        }

        
        public List<AssetEvaluationSummarizedRecord> Records { get; set; }

    }

    public class AssetEvaluationSummarizedRecord : NotifiesPropertyChanged
    {
        private readonly PurchasedAssetEvaluationSummarizedStatement _statement;

        public AssetEvaluationSummarizedRecord(PurchasedAssetEvaluationSummarizedStatement statement)
        {
            Name = statement.Name;
            Quantity = statement.Quantity;
            PurchaseStartDate = statement.PurchaseStartDate;
            PurchaseEndDate = statement.PurchaseEndDate;
            Value = statement.Value;
            ValuePerUnit = statement.GetAverageValue();
            RealizedProfit = statement.RealizedProfit;



            _statement = statement;
            _statement.EvaluationChanged += () =>
            {
                FirePropertyChanged(nameof(CurrentValue));
                FirePropertyChanged(nameof(CurrentValuePerUnit));
                FirePropertyChanged(nameof(UnrealizedProfit));
            };
        }

        public List<string> GetAllPublicPropertyNames()
        {
            var properties = this.GetType().GetProperties();
            return properties.Select(x => x.Name).ToList();
        }

        public string Name { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.DateDisplayFormat)]
        [DisplayName("First Purchase Date")]
        public DateTime PurchaseStartDate { get; set; }

        [DisplayName("Last Purchase Date")]
        [DisplayFormat(DataFormatString = CommonDefinition.DateDisplayFormat)]
        public DateTime PurchaseEndDate { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.QuantityDisplayFormat)]
        public double Quantity { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Cost/Unit")]
        public double ValuePerUnit { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Cost")]
        public double Value { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [Editable(true)]
        [DisplayName("Current Price/Unit")]
        public double? CurrentValuePerUnit
        {
            get { return _statement.QuotePerUnit; }
            set
            {
                _statement.QuotePerUnit = value;
            }
        }

        [DisplayName("Current Price")]
        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        public double? CurrentValue
        {
            get { return _statement.GetCurrentValue(); }
            set { }
        }

        [DisplayName("Realized Profit")]
        public double RealizedProfit { get; set; }


        [DisplayName("Unrealized Profit")]
        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        public double? UnrealizedProfit
        {
            get { return _statement.GetUnrealizedProfit(); }
            set { }
        }
        
    }


}