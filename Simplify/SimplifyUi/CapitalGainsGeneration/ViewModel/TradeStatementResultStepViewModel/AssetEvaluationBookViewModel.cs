using System.Collections.Generic;
using Simplify.Trade;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class AssetEvaluationBookViewModel
    {
        public AssetEvaluationBookViewModel(AssetEvalutionBook book)
        {
            Statements = book.Statements;
            TotalCostOfOpenPosition = book.TotalCostOfOpenPosition;
            CurrentValueOfOpenPosition = book.CurrentValueOfOpenPosition;
            UnrealizedProfit = book.UnrealizedProfit;
        }

        public List<AssetEvaluationStatement> Statements { get; set; }

        public double TotalCostOfOpenPosition { get; set; }

        public double? CurrentValueOfOpenPosition { get; set; }

        public double? UnrealizedProfit { get; set; }

    }
}