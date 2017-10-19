using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Simplify.Trade;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class AssetNamesViewModel
    {
        public AssetNamesViewModel(ProcessedTradeStatementsContainer container)
        {
            AssetNames = new List<AssetNameTableRecord>();
            AssetNames.AddRange(container.AssetNamesBook.Select(x => new AssetNameTableRecord { AssetName = x }));
        }
        public List<AssetNameTableRecord> AssetNames { get; set; }
    }

    public class AssetNameTableRecord
    {
        [DisplayName("Asset Name")]
        public string AssetName { get; set; }
    }
}