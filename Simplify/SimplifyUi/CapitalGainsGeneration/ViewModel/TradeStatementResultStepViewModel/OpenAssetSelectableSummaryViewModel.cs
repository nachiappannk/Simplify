using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Trade;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel.TradeStatementResultStepViewModel
{
    public class OpenAssetSelectableSummaryViewModel : AssetSelectableSummaryViewModel
    {
        protected readonly Dictionary<string, AssetSummaryRecordsAndAssetEvalutaionSummarizedRecords> _dictionary;
        
        public OpenAssetSelectableSummaryViewModel(ProcessedTradeStatementsContainer container)
        {
            _dictionary = new Dictionary<string, AssetSummaryRecordsAndAssetEvalutaionSummarizedRecords>();
            foreach (var openAssetSummaryBook in container.OpenAssetSummaryBooks)
            {
                var records= openAssetSummaryBook.Value.Select(x => new AssetSummaryRecord(x)).ToList();
                var summaryRecords =  container.PurchasedAssetSummarizedStatements
                                    .Where(x => x.Name == openAssetSummaryBook.Key)
                                    .Select(x => new AssetEvaluationSummarizedRecord(x))
                                    .ToList();
                var y = new AssetSummaryRecordsAndAssetEvalutaionSummarizedRecords()
                {
                    Records = records,
                    SummaryRecords = summaryRecords,
                };
                _dictionary.Add(openAssetSummaryBook.Key, y);
            }
            AssetNames = _dictionary.Keys.ToList();


        }

        public List<AssetEvaluationSummarizedRecord> SummaryRecords { get; set; }

        public List<AssetSummaryRecord> Records { get; set; }

        protected override void OnAssetSelectedChanged(string selectedAsset)
        {
            var recordsAndSummaryRecords = _dictionary[selectedAsset];
            Records = recordsAndSummaryRecords.Records;
            SummaryRecords = recordsAndSummaryRecords.SummaryRecords;
            FirePropertyChanged(nameof(SummaryRecords));
            FirePropertyChanged(nameof(Records));
        }
    }

    public class AssetSummaryRecordsAndAssetEvalutaionSummarizedRecords
    {
        public List<AssetEvaluationSummarizedRecord> SummaryRecords { get; set; }

        public List<AssetSummaryRecord> Records { get; set; }
    }

}