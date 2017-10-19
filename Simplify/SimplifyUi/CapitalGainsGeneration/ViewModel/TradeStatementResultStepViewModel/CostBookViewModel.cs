using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Simplify.Trade;
using SimplifyUi.Common;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class CostBookViewModel
    {
        public CostBookViewModel(ProcessedTradeStatementsContainer container)
        {
            CostBookRecords = new List<CostBookRecord>();
            CostBookRecords.AddRange(container.EffectiveCostStatementBook.Select(x => new CostBookRecord()
            {
                Name = x.Name,
                AverageCost = x.AverageCost.ToStringWithNumberOfDecimals(2),
            }));
        }
        public List<CostBookRecord> CostBookRecords { get; set; }
    }

    public class CostBookRecord
    {
        public string Name { get; set; }

        [DisplayName("Average Cost")]
        public string AverageCost { get; set; }
    }
}