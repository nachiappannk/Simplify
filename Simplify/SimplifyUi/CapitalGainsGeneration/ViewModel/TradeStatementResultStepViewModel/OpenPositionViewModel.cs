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
    public class OpenPositionViewModel
    {
        public List<OpenPositionTableRecord> OpenPositions { get; set; }
        public OpenPositionViewModel(ProcessedTradeStatementsContainer container)
        {
            OpenPositions = new List<OpenPositionTableRecord>();
            OpenPositions.AddRange(container.PurchasedAssetEvaluationStatements.Select(x => new OpenPositionTableRecord(x)
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

    public class OpenPositionTableRecord : NotifiesPropertyChanged
    {
        private readonly PurchasedAssetEvaluationStatement _statement;

        public OpenPositionTableRecord(PurchasedAssetEvaluationStatement statement)
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
        public double Cost { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Cost/Unit")]
        public double CostPerUnit { get; set; }


        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Current Price")]
        public double? CurrentValue
        {
            get { return _statement.GetCurrentValue(); }
            set { }
        }

        [Editable(true)]
        [DisplayName("Current Price/Unit")]
        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        public double? CurrentPerUnit {
            get { return _statement.QuotePerUnit; }
            set { _statement.QuotePerUnit = value; } 
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