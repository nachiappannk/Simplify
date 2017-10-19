using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Simplify.Trade;
using SimplifyUi.Common;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class OpenPositionViewModel
    {
        public List<OpenPositionTableRecord> OpenPositions { get; set; }
        public OpenPositionViewModel(ProcessedTradeStatementsContainer container)
        {
            OpenPositions = new List<OpenPositionTableRecord>();
            OpenPositions.AddRange(container.OpenPositionBook.Select(x => new OpenPositionTableRecord
            {
                Date = x.Date.ToStringDisplayable(),
                Name = x.Name,
                Quantity = x.Quantity,
                Cost = x.Value,
                TransactionTax = x.TransactionTax,
                TransactionDetail = x.TransactionDetail,
            }));
        }
    }

    public class OpenPositionTableRecord
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Cost { get; set; }

        [DisplayName("Transaction Detail")]
        public string TransactionDetail { get; set; }

        [DisplayName("Transaction Tax")]
        public string TransactionTax { get; set; }


    }
}