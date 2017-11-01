using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Simplify.Trade;
using SimplifyUi.Annotations;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class AssetEvaluationBookViewModel
    {
        private readonly AssetEvalutionBook _book;

        public AssetEvaluationBookViewModel(AssetEvalutionBook book)
        {
            TotalCostOfOpenPosition = new ViewModelProperty<double>();
            CurrentValueOfOpenPosition = new ViewModelProperty<double?>();
            UnrealizedProfit = new ViewModelProperty<double?>();

            _book = book;
            Statements = book.Statements.Select(y => new AssetEvaluationRecord(y)).ToList();
            foreach (var statement in book.Statements)
            {
                statement.Changed += InitializeCommonProperties;
            }
            InitializeCommonProperties();
        }

        private void InitializeCommonProperties()
        {
            TotalCostOfOpenPosition.Property = _book.TotalCostOfOpenPosition;
            CurrentValueOfOpenPosition.Property = _book.CurrentValueOfOpenPosition;
            UnrealizedProfit.Property = _book.UnrealizedProfit;
        }

        public List<AssetEvaluationRecord> Statements { get; set; }

        public ViewModelProperty<double> TotalCostOfOpenPosition { get; set; }

        public ViewModelProperty<double?> CurrentValueOfOpenPosition { get; set; }

        public ViewModelProperty<double?> UnrealizedProfit { get; set; }

    }

    public class AssetEvaluationRecord : INotifyPropertyChanged
    {
        private readonly AssetEvaluationStatement _statement;

        public AssetEvaluationRecord(AssetEvaluationStatement statement)
        {
            _statement = statement;
            _statement.Changed += () =>
            {
                this.InitializeFromAssetEvaluationStatement(_statement);
                var propertyNames = GetAllPublicPropertyNames();
                foreach (var propertyName in propertyNames)
                {
                    FirePropertyChanged(propertyName);
                }
            };
            this.InitializeFromAssetEvaluationStatement(statement);
        }

        public List<string> GetAllPublicPropertyNames()
        {
            var properties = this.GetType().GetProperties();
            return properties.Select(x => x.Name).ToList();
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
        public double ValuePerUnit { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Cost")]
        public double Value { get; set; }

        private double? currentValuePerUnit;

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [Editable(true)]
        [DisplayName(@"Current Price/Unit")]
        public double? CurrentValuePerUnit
        {
            get { return currentValuePerUnit; }
            set
            {
                if ((value == null) && (currentValuePerUnit == null)) return;
                if (value.HasValue && currentValuePerUnit.HasValue &&
                    Math.Abs(value.Value - currentValuePerUnit.Value) < 0.001) return;
                {
                    currentValuePerUnit = value;
                    _statement.CurrentValuePerUnit = currentValuePerUnit;
                }
            }
        }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Current Price")]
        public double? CurrentValue { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Unrealized Profit")]
        public double? UnrealizedProfit { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void FirePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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