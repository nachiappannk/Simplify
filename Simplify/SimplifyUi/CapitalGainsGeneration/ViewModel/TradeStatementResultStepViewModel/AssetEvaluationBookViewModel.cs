using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Simplify.Trade;
using SimplifyUi.Annotations;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class AssetEvaluationBookViewModel
    {
        public AssetEvaluationBookViewModel(AssetEvalutionBook book)
        {
            Statements = book.Statements.Select(y => new AssetEvaluationRecord(y)).ToList();
            TotalCostOfOpenPosition = book.TotalCostOfOpenPosition;
            CurrentValueOfOpenPosition = book.CurrentValueOfOpenPosition;
            UnrealizedProfit = book.UnrealizedProfit;
        }

        public List<AssetEvaluationRecord> Statements { get; set; }

        public double TotalCostOfOpenPosition { get; set; }

        public double? CurrentValueOfOpenPosition { get; set; }

        public double? UnrealizedProfit { get; set; }

    }

    public class AssetEvaluationRecord
    {
        public AssetEvaluationRecord(AssetEvaluationStatement statement)
        {
            this.InitializeFromAssetEvaluationStatement(statement);
        }

        [DisplayFormat(DataFormatString = "dd/MMM/yyy")]
        public DateTime Date { get; set; }
        public string Name { get; set; }

        [DisplayName("Transaction Detail")]
        public string TransactionDetail { get; set; }

        [DisplayName("Transaction Tax")]
        public string TransactionTax { get; set; }

        [DisplayFormat(DataFormatString = "#.###")]
        public double Quantity { get; set; }

        [DisplayFormat(DataFormatString = "N2")]
        [DisplayName("Cost/Unit")]
        public double ValuePerUnit { get; set; }

        [DisplayFormat(DataFormatString = "N2")]
        [DisplayName("Cost")]
        public double Value { get; set; }

        [DisplayFormat(DataFormatString = "N2")]
        [Editable(true)]
        [DisplayName(@"Current Price/Unit")]
        public double? CurrentValuePerUnit { get; set; }

        [DisplayFormat(DataFormatString = "N2")]
        [DisplayName("Current Price")]
        public double? CurrentValue { get; set; }

        [DisplayFormat(DataFormatString = "N2")]
        [DisplayName("Unrealized Profit")]
        public double? UnrealizedProfit { get; set; }
    }

    public static class AssetEvaluationRecordExtentions
    {
        public static void InitializeFromAssetEvaluationStatement(this AssetEvaluationRecord record,
            AssetEvaluationStatement statement)
        {
            record.Name = statement.Name;
            record.Date = statement.Date;
            record.Quantity = statement.Quantity;
            record.TransactionDetail = statement.TransactionDetail;
            record.TransactionTax = statement.TransactionTax;
            record.Value = statement.Value;
            record.CurrentValuePerUnit = statement.CurrentValuePerUnit;
            record.ValuePerUnit = statement.GetValuePerUnit();
            record.UnrealizedProfit = statement.GetUnrealizedProfit();
            record.CurrentValue = statement.GetCurrentValue();
        }
    }


    public class ViewModelProperty<T> : INotifyPropertyChanged
    {
        private T t;

        public T Property
        {
            get { return t; }
            set
            {
                if (t == null && (value == null)) return;
                if (value == null || !value.Equals(t))
                {
                    t = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}