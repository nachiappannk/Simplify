using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Simplify.Trade;
using SimplifyUi.Common;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class AssetEvaluationViewModel
    {
        public List<AssetEvaluationRecord> OpenPositions { get; set; }
        public AssetEvaluationViewModel(ProcessedTradeStatementsContainer container)
        {
            OpenPositions = new List<AssetEvaluationRecord>();
            OpenPositions.AddRange(container.PurchasedAssetEvaluationStatements.Select(x => new AssetEvaluationRecord(x)
            {
                Date = x.Date,
                Name = x.Name,
                Quantity = x.Quantity,
                Cost = x.Value,
                TransactionTax = x.TransactionTax,
                TransactionDetail = x.TransactionDetail,
                CostPerUnit = x.GetAverageValue(),
            }));
        }
    }

    public class AssetEvaluationRecord : NotifiesPropertyChanged
    {
        private readonly PurchasedAssetEvaluationStatement _statement;

        public AssetEvaluationRecord(PurchasedAssetEvaluationStatement statement)
        {
            _statement = statement;
            _statement.EvaluationChanged += OnStatementEvaluationChanged;
        }

        private void OnStatementEvaluationChanged()
        {
            FirePropertyChanged(nameof(UnrealizedProfit));
            FirePropertyChanged(nameof(CurrentValue));
            FirePropertyChanged(nameof(CurrentPerUnit));
        }

        [DisplayFormat(DataFormatString = CommonDefinition.DateDisplayFormat)]
        public DateTime Date { get; set; }
        public string Name { get; set; }


        [DisplayName("Transaction Detail")]
        public string TransactionDetail { get; set; }

        [DisplayName("Transaction Tax")]
        public string TransactionTax { get; set; }


        [DisplayFormat(DataFormatString = CommonDefinition.QuantityDisplayFormat)]
        public double Quantity { get; set; }


        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Cost/Unit")]
        public double CostPerUnit { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        public double Cost { get; set; }

        [Editable(true)]
        [DisplayName("Current Price/Unit")]
        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        public double? CurrentPerUnit
        {
            get { return _statement.QuotePerUnit; }
            set { _statement.QuotePerUnit = value; }
        }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Current Price")]
        public double? CurrentValue
        {
            get { return _statement.GetCurrentValue(); }
            set { }
        }

        [DisplayName("Unrealized Profit")]
        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        public double? UnrealizedProfit
        {
            get { return _statement.GetUnrealizedProfit(); }
            set { }
        }


    }
}