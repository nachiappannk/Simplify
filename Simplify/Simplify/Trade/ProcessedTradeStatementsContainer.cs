using System;
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

    public class AssetQuotation
    {
        public string Name { get; set; }
        public bool IsClosed { get; set; }
        public double? QuotePerUnit { get; set; }
    }

    public class PurchasedAssetEvaluationStatement
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }
        public string TransactionTax { get; set; }
        public string TransactionDetail { get; set; }
        public double? QuotePerUnit { get; set; }
    }

    public class PurchasedAssetEvaluationSummarizedStatement
    {
        public string Name { get; set; }
        public DateTime PurchaseStartDate { get; set; }
        public DateTime PurchaseEndDate { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }
        public double? QuotePerUnit { get; set; }
    }

    public class ProcessedTradeStatementsContainer
    {
        public List<string> AssetNamesBook { get; private set; }
        public List<TradeStatement> OpenPositionBook { get; private set; }
        public List<SquarableStatement> ProfitBook { get; set; }
        public Dictionary<string, OpenAssetSummaryBook> OpenAssetSummaryBooks { get; private set; }
        public Dictionary<string, ClosedAssetSummaryBook> ClosedAssetSummaryBooks { get; private set; }
        public AssetEvalutionBook AssetEvalutionBook { get; set; }

        public AssetEvaluationAggregatedBook AssetEvaluationAggregatedBook { get; set; }

        private readonly QuotationRepository _repository;

        public ProcessedTradeStatementsContainer(List<TradeStatement> tradeStatements, List<QuotationStatement> quotationStatements)
        {
            _repository = new QuotationRepository(quotationStatements);
            InitializeAssetNamesBook(tradeStatements);
            InitializeOpenPositionAndProfitBook(tradeStatements);
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



        private void InitializeOpenPositionAndProfitBook(List<TradeStatement> tradeStatements)
        {
            var separator = new StatementsSeparator();
            List<TradeStatement> openStatements;
            List<SquarableStatement> closedStatements;
            separator.SeparateStatementsAsClosedAndOpen(tradeStatements, out openStatements, out closedStatements);
            OpenPositionBook = openStatements;
            ProfitBook = closedStatements;
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
                    book.Statements = new List<SquarableStatement>();
                    book.Statements.AddRange(bookStatements);
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