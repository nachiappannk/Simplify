using Simplify.Trade;
using SimplifyUi.Common.ViewModel;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class SummaryViewModel : NotifiesPropertyChanged
    {
        private readonly TradeSummary _summary;

        public SummaryViewModel(TradeSummary summary)
        {
            _summary = summary;
            _summary.Changed += () =>
            {
                FirePropertyChanged(nameof(Profit));
                FirePropertyChanged(nameof(CostOfOpenPosition));
                FirePropertyChanged(nameof(ValueOfOpenPosition));
                FirePropertyChanged(nameof(UnrealizedProfit));
            };
        }

        public double Profit
        {
            get { return _summary.Profit; }
            set { }
        }

        public double CostOfOpenPosition
        {
            get { return _summary.CostOfOpenPosition; }
            set { }
        }

        public double? UnrealizedProfit
        {
            get { return _summary.UnrealizedProfit; }
            set { }
        }

        public double? ValueOfOpenPosition
        {
            get { return _summary.ValueOfOpenPosition; }
            set { }
        }
    }
}