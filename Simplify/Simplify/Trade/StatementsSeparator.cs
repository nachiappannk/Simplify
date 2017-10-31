using System.Collections.Generic;
using System.Linq;

namespace Simplify.Trade
{
    public class StatementsSeparator
    {
        public List<TradeStatement> OpenPositionBook { get; private set; }
        public List<SquarableStatement> ProfitBook { get; set; }

        public void SeparateStatementsAsClosedAndOpen(List<TradeStatement> statements, out List<TradeStatement> openStatements, out List<SquarableStatement> closedStatements)
        {
            OpenPositionBook = new List<TradeStatement>();
            ProfitBook = new List<SquarableStatement>();
            SeparateStatementsAsClosedAndOpen(statements);
            openStatements = OpenPositionBook;
            closedStatements = ProfitBook;
        }

        private void SeparateStatementsAsClosedAndOpen(List<TradeStatement> tradeStatements)
        {
            var assetNames = tradeStatements.Select(x => x.Name).Distinct().OrderBy(x => x).ToList();

            foreach (var name in assetNames)
            {
                ProcessTradeStatements(tradeStatements.Where(x => x.Name == name).ToList());
            }
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
            var squarableStatement = new SquarableStatement(new[] { brokenStatements.BrokenTradeStatement, smaller });
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
                if (remainingStatement.Quantity > 0.0001)
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