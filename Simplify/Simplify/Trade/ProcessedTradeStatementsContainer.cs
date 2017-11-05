using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Simplify.CommonDefinitions;

namespace Simplify.Trade
{

    public class TradeSummary
    {
        public double CostOfOpenPosition { get; set; }
        public double Profit { get; set; }
        public double? ValueOfOpenPosition { get; set; }
        public double? UnrealizedProfit { get; set; }
    }

    public class ProcessedTradeStatementsContainer
    {
        private List<SquarableStatement> _purchaseAndSaleMappedStatements;
        private Dictionary<string,List<SquarableStatement>> statementsForAssets;
        
        public List<AssetQuotation> AssetQuotations { get; set; }
        public List<PurchasedAssetEvaluationStatement> PurchasedAssetEvaluationStatements { get; set; }

        public List<PurchasedAssetEvaluationSummarizedStatement> PurchasedAssetSummarizedStatements { get; set;}

        public List<string> AssetNamesBook { get; private set; }
        public List<TradeStatement> OpenPositionBook { get; private set; }
        public List<SquarableStatement> ProfitBook { get; set; }
        public Dictionary<string, List<SquarableStatement>> OpenAssetSummaryBooks { get; private set; }
        public Dictionary<string, ClosedAssetSummaryBook> ClosedAssetSummaryBooks { get; private set; }
        public AssetEvalutionBook AssetEvalutionBook { get; set; }

        public AssetEvaluationAggregatedBook AssetEvaluationAggregatedBook { get; set; }

        private readonly QuotationRepository _repository;

        public ProcessedTradeStatementsContainer(List<TradeStatement> tradeStatements, List<QuotationStatement> quotationStatements)
        {
            _repository = new QuotationRepository(quotationStatements);

            var assetNames = tradeStatements.Select(x => x.Name).Distinct().ToList();
            assetNames.AddRange(quotationStatements.Select(x => x.Name));
            assetNames = assetNames.Distinct().ToList();

            _purchaseAndSaleMappedStatements = GetSaleAndPurchaseMappedStatements(tradeStatements);

            PurchasedAssetEvaluationStatements = _purchaseAndSaleMappedStatements
                .Where(x => !x.IsSquared)
                .Select(x => x.CreatePurchasedAssetEvalutionStatement(_repository.GetQuote(x.Name)))
                .ToList();

            PurchasedAssetSummarizedStatements = PurchasedAssetEvaluationStatements
                .GroupBy(x => x.Name, x => x, (name, statements) =>
                {
                    var result = new PurchasedAssetEvaluationSummarizedStatement(_repository.GetQuote(name));
                    result.Name = name;
                    var statementList = statements.ToList();
                    result.PurchaseStartDate = statementList.Select(x => x.Date).Min();
                    result.PurchaseEndDate = statementList.Select(x => x.Date).Max();
                    result.Quantity = statementList.Select(x => x.Quantity).Sum();
                    result.Value = statementList.Select(x => x.Value).Sum();
                    return result;
                }).ToList();

            

            var groupedStatements = _purchaseAndSaleMappedStatements.GroupBy(x => x.Name, x=> x);
            statementsForAssets =  groupedStatements.ToDictionary(x => x.Key, x => x.AsEnumerable().ToList());

            var statementsForClosedAssets = new Dictionary<string, List<SquarableStatement>>();
            var statementsForOpenAssets = new Dictionary<string, List<SquarableStatement>>();
            foreach (var statementsForAsset in statementsForAssets)
            {
                if (statementsForAsset.Value.Any(x => !x.IsSquared))
                    statementsForOpenAssets.Add(statementsForAsset.Key, statementsForAsset.Value);
                else
                    statementsForClosedAssets.Add(statementsForAsset.Key, statementsForAsset.Value);
            }

            PurchasedAssetSummarizedStatements = statementsForOpenAssets
                .Select(y =>
                {
                    var name = y.Key;
                    var statements = y.Value;
                    var openStatements = statements.Where(x => !x.IsSquared).ToList();
                    var closedStatements = statements.Where(x => x.IsSquared).ToList();
                    var result =
                        new PurchasedAssetEvaluationSummarizedStatement(_repository.GetQuote(name))
                        {
                            Name = name,
                            PurchaseStartDate = openStatements.Select(x => x.PurchaseDate).Min(),
                            PurchaseEndDate = openStatements.Select(x => x.PurchaseDate).Max(),
                            Quantity = openStatements.Select(x => x.Quantity).Sum(),
                            Value = openStatements.Select(x => x.PurchaseValue).Sum(),
                            RealizedProfit = closedStatements.Sum(x => x.SaleValue - x.PurchaseValue)
                        };
                    return result;
                }).ToList();

            AssetQuotations = new List<AssetQuotation>();
            HashSet<string> assetChecker = new HashSet<string>();
            foreach (var quotationStatement in quotationStatements)
            {
                if (assetChecker.Contains(quotationStatement.Name)) continue;
                var assetQuotation = new AssetQuotation(_repository.GetQuote(quotationStatement.Name))
                {
                    Name = quotationStatement.Name,
                    IsOwned = statementsForOpenAssets.ContainsKey(quotationStatement.Name)
                };
                assetChecker.Add(quotationStatement.Name);
                AssetQuotations.Add(assetQuotation);
            }
            foreach (var statementsForClosedAsset in statementsForClosedAssets)
            {
                var assetName = statementsForClosedAsset.Key;
                if(assetChecker.Contains(assetName))continue;
                var assetQuotation = new AssetQuotation(_repository.GetQuote(assetName))
                {
                    Name = assetName,
                    IsOwned = false,
                };
                assetChecker.Add(assetName);
                AssetQuotations.Add(assetQuotation);
            }
            foreach (var statementsForOpenAsset in statementsForOpenAssets)
            {
                var assetName = statementsForOpenAsset.Key;
                if (assetChecker.Contains(assetName)) continue;
                var assetQuotation = new AssetQuotation(_repository.GetQuote(assetName))
                {
                    Name = assetName,
                    IsOwned = true,
                };
                assetChecker.Add(assetName);
                AssetQuotations.Add(assetQuotation);
            }


            ProfitBook = new List<SquarableStatement>();
            ProfitBook.AddRange(_purchaseAndSaleMappedStatements.Where(x => x.IsSquared));

            OpenPositionBook = _purchaseAndSaleMappedStatements.Where(x => !x.IsSquared).Select(x => x.ConvertToTradeStatement()).ToList();

            InitializeAssetNamesBook(tradeStatements);
            InitializeSummaryBooks();
            InitializeAssetEvaluationBook(OpenPositionBook);
            InitializeAssetEvaluationAggregatedBook(OpenPositionBook);
        }

        private void InitializeAssetEvaluationAggregatedBook(List<TradeStatement> openPositionBook)
        {
            AssetEvaluationAggregatedBook = new AssetEvaluationAggregatedBook();
            var result = openPositionBook.GroupBy(x => x.Name, x => x, (name, statements) =>
            {
                var statementList = statements.ToList();
                var r = new AssetEvaluationAggregatedStatement(_repository.GetQuote(name))
                {
                    Name = name,
                    Value = statementList.Sum(x => x.Value),
                    Quantity = statementList.Sum(x => x.Quantity),
                    PurchaseStartDate = statementList.Select(x => x.Date).Min(),
                    PurchaseEndDate = statementList.Select(x => x.Date).Max()
                };
                return r;
            });
            AssetEvaluationAggregatedBook.Statements = result.ToList();
        }

        private void InitializeAssetEvaluationBook(List<TradeStatement> openPositionBook)
        {
            AssetEvalutionBook = new AssetEvalutionBook();
            List<AssetEvaluationStatement> statements = new List<AssetEvaluationStatement>();
            AssetEvalutionBook.Statements = statements;
            foreach (var openPosition in openPositionBook)
            {
                var assetEvaluationStatement = new AssetEvaluationStatement(_repository.GetQuote(openPosition.Name));
                assetEvaluationStatement.InitializeFromTradeStatement(openPosition);
                statements.Add(assetEvaluationStatement);
            }
        }



        private List<SquarableStatement> GetSaleAndPurchaseMappedStatements(List<TradeStatement> tradeStatements)
        {
            var mapper = new SaleAndPurchaseMapper();
            var mappedStatements = mapper.MapSaleAndPurchaseStatements(tradeStatements);
            return mappedStatements;
        }

        private void InitializeSummaryBooks()
        {
            var allStatements = ProfitBook.ToList();
            allStatements.AddRange(OpenPositionBook.Select(x => new SquarableStatement(x)));
            OpenAssetSummaryBooks = new Dictionary<string, List<SquarableStatement>>();
            ClosedAssetSummaryBooks = new Dictionary<string, ClosedAssetSummaryBook>();
            foreach (var name in AssetNamesBook)
            {
                var bookStatements = allStatements.Where(x => x.Name.Equals(name)).ToList();
                if (bookStatements.Any(x => x.IsSquared == false))
                {
                    var statements = new List<SquarableStatement>();
                    statements.AddRange(bookStatements);
                    OpenAssetSummaryBooks.Add(name, statements);
                }
                else
                {
                    var book = new ClosedAssetSummaryBook();
                    book.AddRange(bookStatements);
                    ClosedAssetSummaryBooks.Add(name, book);
                }
            }
        }

        private void InitializeAssetNamesBook(List<TradeStatement> tradeStatements)
        {
            AssetNamesBook = tradeStatements.Select(x => x.Name).Distinct().OrderBy(x => x).ToList();
        }


        
    }
}