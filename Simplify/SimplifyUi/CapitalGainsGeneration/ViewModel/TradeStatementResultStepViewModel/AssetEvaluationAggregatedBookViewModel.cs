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
    public class AssetEvaluationAggregatedBookViewModel
    {
        private readonly AssetEvaluationAggregatedBook _book;

        public AssetEvaluationAggregatedBookViewModel(AssetEvaluationAggregatedBook book)
        {

            TotalCostOfOpenPosition = new ViewModelDoubleProperty();
            CurrentValueOfOpenPosition = new ViewModelNullableDoubleProperty();
            UnrealizedProfit = new ViewModelNullableDoubleProperty();

            _book = book;

            Records = book.Statements.Select(x => new AssetEvaluationAggregatedRecord(x)).ToList();
            foreach (var statement in book.Statements)
            {
                statement.Changed += InitializeProperties;
            }
            InitializeProperties();
        }

        private void InitializeProperties()
        {
            TotalCostOfOpenPosition.Property = _book.TotalCostOfOpenPosition;
            CurrentValueOfOpenPosition.Property = _book.CurrentValueOfOpenPosition;
            UnrealizedProfit.Property = _book.UnrealizedProfit;
        }

        public List<AssetEvaluationAggregatedRecord> Records { get; set; }

        public ViewModelDoubleProperty TotalCostOfOpenPosition { get; set; }

        public ViewModelNullableDoubleProperty CurrentValueOfOpenPosition { get; set; }

        public ViewModelNullableDoubleProperty UnrealizedProfit { get; set; }
    }

    public class AssetEvaluationAggregatedRecord : INotifyPropertyChanged
    {
        private readonly AssetEvaluationAggregatedStatement _statement;
        public event PropertyChangedEventHandler PropertyChanged;


        public AssetEvaluationAggregatedRecord(AssetEvaluationAggregatedStatement statement)
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


        public DateTime PurchaseStartDate { get; set; }
        public DateTime PurchaseEndDate { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }

        private double? _currentValuePerUnit;

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [Editable(true)]
        [DisplayName(@"Current Price/Unit")]
        public double? CurrentValuePerUnit
        {
            get { return _currentValuePerUnit; }
            set
            {
                if (_currentValuePerUnit.IsNullableDoubleEqual(value)) return;
                _currentValuePerUnit = value;
                _statement.CurrentValuePerUnit = _currentValuePerUnit;

            }
        }

        public double ValuePerUnit { get; set; }
        public double? CurrentValue { get; set; }

        public double? UnrealizedProfit { get; set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void FirePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class AssetEvaluationAggregatedRecordExtentions
    {
        public static void InitializeFromAssetEvaluationStatement(this AssetEvaluationAggregatedRecord record,
            AssetEvaluationAggregatedStatement statement)
        {
            record.Name = statement.Name;
            record.CurrentValue = statement.GetCurrentValue();
            record.CurrentValuePerUnit = statement.CurrentValuePerUnit;
            record.Value = statement.Value;
            record.ValuePerUnit = statement.GetValuePerUnit();
            record.Quantity = statement.Quantity;
            record.PurchaseStartDate = statement.PurchaseStartDate;
            record.PurchaseEndDate = statement.PurchaseEndDate;
            record.UnrealizedProfit = statement.GetUnrealizedProfit();
        }
    }
}