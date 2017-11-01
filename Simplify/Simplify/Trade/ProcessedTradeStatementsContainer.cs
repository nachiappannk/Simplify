using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Simplify.CommonDefinitions;

namespace Simplify.Trade
{
    public class ProcessedTradeStatementsContainer
    {
        public List<string> AssetNamesBook { get; private set; }
        public List<TradeStatement> OpenPositionBook { get; private set; }
        public List<SquarableStatement> ProfitBook { get; set; }
        public Dictionary<string, OpenAssetSummaryBook> OpenAssetSummaryBooks { get; private set; }
        public Dictionary<string, ClosedAssetSummaryBook> ClosedAssetSummaryBooks { get; private set; }
        public List<CostStatement> EffectiveCostStatementBook { get; private set; }
        public AssetEvalutionBook AssetEvalutionBook { get; set; }

        private QuotationRepository repository;

        public ProcessedTradeStatementsContainer(List<TradeStatement> tradeStatements, List<QuotationStatement> quotationStatements)
        {
            repository = new QuotationRepository(quotationStatements);
            InitializeAssetNamesBook(tradeStatements);
            InitializeOpenPositionAndProfitBook(tradeStatements);
            InitializeSummaryBooks();
            InitializeEffectiveCostStatementBook();
            InitializeAssetEvaluationBook(OpenPositionBook);
        }

        private void InitializeAssetEvaluationBook(List<TradeStatement> openPositionBook)
        {
            AssetEvalutionBook = new AssetEvalutionBook();
            List<AssetEvaluationStatement> statements = new List<AssetEvaluationStatement>();
            AssetEvalutionBook.Statements = statements;
            foreach (var openPosition in openPositionBook)
            {
                var assetEvaluationStatement = new AssetEvaluationStatement(repository.GetQuote(openPosition.Name));
                assetEvaluationStatement.InitializeFromTradeStatement(openPosition);
                statements.Add(assetEvaluationStatement);
            }
        }



        private void InitializeOpenPositionAndProfitBook(List<TradeStatement> tradeStatements)
        {
            var separator = new StatementsSeparator();
            List<TradeStatement> openStatements;
            List<SquarableStatement> closedStatements;
            separator.SeparateStatementsAsClosedAndOpen(tradeStatements, out openStatements, out closedStatements);
            OpenPositionBook = openStatements;
            ProfitBook = closedStatements;
        }

        private void InitializeEffectiveCostStatementBook()
        {
            EffectiveCostStatementBook = new List<CostStatement>();
            var openPositionBookHeads = OpenPositionBook.Select(x => x.Name).Distinct().ToList();

            foreach (var head in openPositionBookHeads)
            {
                var statements = OpenPositionBook.Where(x => x.Name.Equals(head)).ToList();
                var sumOfValues = statements.Select(x => x.Value).Sum();
                var sumOfQuantities = statements.Select(x => x.Quantity).Sum();
                EffectiveCostStatementBook.Add(new CostStatement() {Name = head, AverageCost = sumOfValues / sumOfQuantities});
            }
        }

        private void InitializeSummaryBooks()
        {
            var allStatements = ProfitBook.ToList();
            allStatements.AddRange(OpenPositionBook.Select(x => new SquarableStatement(x)));
            OpenAssetSummaryBooks = new Dictionary<string, OpenAssetSummaryBook>();
            ClosedAssetSummaryBooks = new Dictionary<string, ClosedAssetSummaryBook>();
            foreach (var name in AssetNamesBook)
            {
                var bookStatements = allStatements.Where(x => x.Name.Equals(name)).ToList();
                if (bookStatements.Any(x => x.IsSquared == false))
                {
                    var book = new OpenAssetSummaryBook();
                    book.AddRange(bookStatements);
                    OpenAssetSummaryBooks.Add(name, book);
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