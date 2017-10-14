using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Books;

namespace Simplify.Application
{
    public class SquaredAndOpenTradeStatements
    {
        public List<SquaredStatement> SquaredStatements { get; set; }
        public List<TradeStatement> OpenTradeStatements { get; set; }
    }

    public class CostStatement
    {
        public string Name { get; set; }
        public double AverageCost { get; set; }
    }

    public class ProcessedTradeStatementsContainer
    {
        public List<string> AssetNamesBook { get; private set; }
        public List<TradeStatement> OpenPositionBook { get; private set; }
        public List<SquarableStatement> ProfitBook { get; set; }
        public List<Dictionary<string, List<SquarableStatement>>> AssetSummaryBooks { get; private set; }
        public List<CostStatement> EffectiveCostStatementBook { get; private set; }


        public ProcessedTradeStatementsContainer(List<TradeStatement> tradeStatements)
        {
            InitializeAssetNamesBook(tradeStatements);
            InitializeOpenPositionAndProfitBook(tradeStatements);
            InitializeAssetSummaryBooks();
            InitializeEffectiveCostStatementBook();
        }

        private void InitializeEffectiveCostStatementBook()
        {
            EffectiveCostStatementBook = new List<CostStatement>();
            var openPositionBookHeads = OpenPositionBook.Select(x => x.Name).Distinct().ToList();

            foreach (var head in openPositionBookHeads)
            {
                var statements = OpenPositionBook.Where(x => x.Name.Equals(head)).ToList();
                var sum = statements.Select(x => x.Value * x.Quantity).Sum();
                var quantity = statements.Select(x => x.Quantity).Sum();
                EffectiveCostStatementBook.Add(new CostStatement() {Name = head, AverageCost = sum / quantity});
            }
        }

        private void InitializeAssetSummaryBooks()
        {
            var allStatements = ProfitBook.ToList();
            allStatements.AddRange(OpenPositionBook.Select(x => new SquarableStatement(x)));
            AssetSummaryBooks = new List<Dictionary<string, List<SquarableStatement>>>();
            foreach (var name in AssetNamesBook)
            {
                var book = new Dictionary<string, List<SquarableStatement>>();
                var bookStatements = allStatements.Where(x => x.Name.Equals(name)).ToList();
                book.Add(name, bookStatements);
                AssetSummaryBooks.Add(book);
            }
        }

        private void InitializeOpenPositionAndProfitBook(List<TradeStatement> tradeStatements)
        {
            OpenPositionBook = new List<TradeStatement>();
            ProfitBook = new List<SquarableStatement>();
            foreach (var name in AssetNamesBook)
            {
                ProcessTradeStatements(tradeStatements.Where(x => x.Name == name).ToList());
            }
        }

        private void InitializeAssetNamesBook(List<TradeStatement> tradeStatements)
        {
            AssetNamesBook = tradeStatements.Select(x => x.Name).Distinct().ToList();
        }
        
        private void ProcessTradeStatements(List<TradeStatement> tradeStatements)
        {
            var purchaseStatements = tradeStatements.Where(x => x.IsPurchase).OrderBy(x => x.Date).ToList();
            var saleStatements = tradeStatements.Where(x => !x.IsPurchase).OrderBy(x => x.Date).ToList();
            while (purchaseStatements.Any() && saleStatements.Any())
            {
                var squaredAndRemaining = SquareStatements(
                    RemoveFirstItem(purchaseStatements),
                    RemoveFirstItem(saleStatements));

                ProfitBook.Add(squaredAndRemaining.SquarableStatement);
                var remaining = squaredAndRemaining.RemainingTradeStatement;
                if (remaining == null) continue;
                if (remaining.IsPurchase)
                    purchaseStatements.Insert(0, remaining);
                else
                    saleStatements.Insert(0, remaining);
            }
            if (purchaseStatements.Any())
                OpenPositionBook.AddRange(purchaseStatements);
            if (saleStatements.Any())
                OpenPositionBook.AddRange(saleStatements);
        }

        private TradeStatement RemoveFirstItem(IList<TradeStatement> statements)
        {
            var statement = statements.ElementAt(0);
            statements.RemoveAt(0);
            return statement;
        }

        private SquarableStatementAndRemainder SquareStatements(TradeStatement ts1, TradeStatement ts2)
        {
            TradeStatement bigger, smaller;
            if (ts1.Quantity > ts2.Quantity)
            {
                bigger = ts1;
                smaller = ts2;
            }
            else
            {
                bigger = ts2;
                smaller = ts1;
            }

            var brokenStatements = new BrokenTradeStatements(bigger, smaller.Quantity);
            var squarableStatement = new SquarableStatement(new [] { brokenStatements.BrokenTradeStatement, smaller });
            return new SquarableStatementAndRemainder(squarableStatement, brokenStatements.RemainingTradeStatement);

        }

        private class BrokenTradeStatements
        {
            public BrokenTradeStatements(TradeStatement ts, double quantity)
            {
                var brokenStatement = CreateACopy(ts);
                var remainingStatement = CreateACopy(ts);
                var initialQuantity = ts.Quantity;
                var initialValue = ts.Value;
                brokenStatement.Quantity = quantity;
                remainingStatement.Quantity = initialQuantity - quantity;
                var brokenValue = initialValue * quantity / initialQuantity;
                brokenStatement.Value = brokenValue;
                remainingStatement.Value = initialValue - brokenValue;
                BrokenTradeStatement = brokenStatement;
                if(remainingStatement.Quantity > 0.0001)
                    RemainingTradeStatement = remainingStatement;
            }

            private static TradeStatement CreateACopy(TradeStatement ts)
            {
                var tradeStatement = new TradeStatement
                {
                    IsPurchase = ts.IsPurchase,
                    Date = ts.Date,
                    Name = ts.Name,
                    TransactionTax = ts.TransactionTax,
                    TransactionDetail = ts.TransactionDetail,
                    Quantity = ts.Quantity,
                    Value = ts.Value,
                };
                return tradeStatement;
            }

            public TradeStatement BrokenTradeStatement { get; private set; }
            public TradeStatement RemainingTradeStatement { get; private set; }
        }

        private class SquarableStatementAndRemainder
        {

            public SquarableStatementAndRemainder(SquarableStatement squarableStatement, TradeStatement remainingTradeStatement)
            {
                SquarableStatement = squarableStatement;
                RemainingTradeStatement = remainingTradeStatement;
            }

            public SquarableStatement SquarableStatement { get; set; }
            public TradeStatement RemainingTradeStatement { get; set; }
        }
    }
}