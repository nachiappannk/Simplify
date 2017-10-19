using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Simplify.Trade;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class ProfitBookViewModel
    {
        public List<ProfitBookTableRecord> ProfitBook { get; set; }
        public ProfitBookViewModel(ProcessedTradeStatementsContainer container)
        {
            ProfitBook = new List<ProfitBookTableRecord>();
            ProfitBook.AddRange(container.ProfitBook.Select(x => new ProfitBookTableRecord()
            {
                Name = x.Name,
                Quantity = x.Quantity,
                PurchaseDate = x.PurchaseDate,
                PurchaseValue = x.PurchaseValue,
                PurchaseTransactionTax = x.PurchaseTransactionTax,
                PurchaseTransactionDetail = x.PurchaseTransactionDetail,
                SaleDate = x.SaleDate,
                SaleValue = x.SaleValue,
                SaleTransactionDetail = x.SaleTransactionDetail,
                SaleTransactionTax = x.SaleTransactionTax,
                Profit = x.GetProfit(),
            }));
        }
    }

    public class ProfitBookTableRecord
    {
        public string Name { get; set; }
        public double Quantity { get; set; }

        [DisplayName("Purchase Date")]
        public DateTime PurchaseDate { get; set; }

        [DisplayName("Sale Date")]
        public DateTime SaleDate { get; set; }

        [DisplayName("Cost")]
        public double PurchaseValue { get; set; }

        [DisplayName("Sale")]
        public double SaleValue { get; set; }
        public double Profit { get; set; }

        [DisplayName("Purchase Transaction Tax")]
        public string PurchaseTransactionTax { get; set; }

        [DisplayName("Purchase Transaction Detail")]
        public string PurchaseTransactionDetail { get; set; }

        [DisplayName("Sale Transaction Tax")]
        public string SaleTransactionTax { get; set; }

        [DisplayName("Sale Transaction Detail")]
        public string SaleTransactionDetail { get; set; }

    }

}