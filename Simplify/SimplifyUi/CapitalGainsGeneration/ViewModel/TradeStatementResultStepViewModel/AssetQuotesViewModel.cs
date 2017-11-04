using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Simplify.Trade;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class AssetQuotesViewModel
    {
        public AssetQuotesViewModel(ProcessedTradeStatementsContainer container)
        {
            Records = container.AssetQuotations.Select(x => new AssetQuoteRecord(x)).ToList();
        }

        public List<AssetQuoteRecord> Records { get; set; }

    }

    public class AssetQuoteRecord : NotifiesPropertyChanged
    {
        private readonly AssetQuotation _assetQuotation;

        public AssetQuoteRecord(AssetQuotation assetQuotation)
        {
            _assetQuotation = assetQuotation;
            assetQuotation.Changed += () => { FirePropertyChanged(nameof(CurrentPrice)); };
            Name = assetQuotation.Name;
            OwnerShipInfo = assetQuotation.IsOwned ? "Open" : "Closed";

        }

        public string Name { get; set; }

        [DisplayName("Status")]
        public string OwnerShipInfo { get; set; }

        [Editable(true)]
        [DisplayName("Current Price")]
        [DisplayFormat(DataFormatString = CommonDefinition.ValueDisplayFormat)]
        public double? CurrentPrice
        {
            get { return _assetQuotation.QuotePerUnit; }
            set { _assetQuotation.QuotePerUnit = value; }
        }
    }
}