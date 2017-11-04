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
                HoldingDays = x.GetNumberOfHoldingDays(),
            }));
        }
    }

    public class ProfitBookTableRecord
    {
        public string Name { get; set; }


        [DisplayName("Purchase TT")]
        public string PurchaseTransactionTax { get; set; }

        [DisplayName("Purchase TD")]
        public string PurchaseTransactionDetail { get; set; }

        [DisplayName("Sale TT")]
        public string SaleTransactionTax { get; set; }

        [DisplayName("Sale TD")]
        public string SaleTransactionDetail { get; set; }


        [DisplayFormat(DataFormatString = CommonDefinition.QuantityDisplayFormat)]
        public double Quantity { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.DateDisplayFormat)]
        [DisplayName("Purchase Date")]
        public DateTime PurchaseDate { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.DateDisplayFormat)]
        [DisplayName("Sale Date")]
        public DateTime SaleDate { get; set; }

        [DisplayName("Held Days")]
        public int HoldingDays { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Cost")]
        public double PurchaseValue { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        [DisplayName("Sale")]
        public double SaleValue { get; set; }

        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        public double Profit { get; set; }


    }

}