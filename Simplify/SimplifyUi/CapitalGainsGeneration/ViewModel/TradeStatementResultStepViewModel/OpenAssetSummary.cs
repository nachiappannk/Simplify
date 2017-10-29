using System.Collections.Generic;
using System.Linq;
using Simplify.Trade;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class OpenAssetSummary : AssetSummary<List<SquarableStatement>>
    {
        public OpenAssetSummary(Dictionary<string, List<SquarableStatement>> dictionary) : base(dictionary)
        {
        }

        protected override void OnAssetSelectedChanged(string selectedAsset)
        {
            var records = _dictionary[selectedAsset];
            Records = records.Select(x => new AssetSummaryRecord(x)).ToList();
        }
    }
}