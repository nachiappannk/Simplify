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

    public class SquaredAndOpenTradeSeparator
    {
        public List<SquaredStatement> SquaredStatements { get; private set; }
        public List<TradeStatement> OpenTradeStatements { get; private set; }
        public SquaredAndOpenTradeStatements Separate(IList<TradeStatement> tradeStatements)
        {
            SquaredStatements = new List<SquaredStatement>();
            OpenTradeStatements = new List<TradeStatement>();
            var names = tradeStatements.Select(x => x.Name).Distinct();
            foreach (var name in names)
            {
                ProcessTradeStatements(tradeStatements.Where(x => x.Name == name));
            }
            return new SquaredAndOpenTradeStatements()
            {
                SquaredStatements = SquaredStatements,
                OpenTradeStatements = OpenTradeStatements,
            };
        }


        private void ProcessTradeStatements(IEnumerable<TradeStatement> tradeStatements)
        {
            var purchaseStatements = tradeStatements.Where(x => x.IsPurchase).OrderBy(x => x.Date).ToList();
            var saleStatements = tradeStatements.Where(x => !x.IsPurchase).OrderBy(x => x.Date).ToList();
            while (purchaseStatements.Any() && saleStatements.Any())
            {
                var squaredAndRemaining = SquareStatements(
                    RemoveFirstItem(purchaseStatements),
                    RemoveFirstItem(saleStatements));

                SquaredStatements.Add(squaredAndRemaining.SquaredStatement);
                var remaining = squaredAndRemaining.RemainingTradeStatement;
                if (remaining == null) continue;
                if (remaining.IsPurchase)
                    purchaseStatements.Insert(0, remaining);
                else
                    saleStatements.Insert(0, remaining);
            }
            if (purchaseStatements.Any())
                OpenTradeStatements.AddRange(purchaseStatements);
            if (saleStatements.Any())
                OpenTradeStatements.AddRange(saleStatements);
        }

        private TradeStatement RemoveFirstItem(IList<TradeStatement> statements)
        {
            var statement = statements.ElementAt(0);
            statements.RemoveAt(0);
            return statement;
        }

        private SquaredStatementAndRemainder SquareStatements(TradeStatement ts1, TradeStatement ts2)
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
            var squaredStatements = new SquaredStatement(brokenStatements.BrokenTradeStatement, smaller);
            return new SquaredStatementAndRemainder(squaredStatements, brokenStatements.RemainingTradeStatement);

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
                    Stt = ts.Stt,
                    Account = ts.Account,
                    Contract = ts.Contract,
                    Quantity = ts.Quantity,
                    ItemType = ts.ItemType,
                    Value = ts.Value,
                };
                return tradeStatement;
            }

            public TradeStatement BrokenTradeStatement { get; private set; }
            public TradeStatement RemainingTradeStatement { get; private set; }
        }

        private class SquaredStatementAndRemainder
        {

            public SquaredStatementAndRemainder(SquaredStatement squaredStatement, TradeStatement remainingTradeStatement)
            {
                SquaredStatement = squaredStatement;
                RemainingTradeStatement = remainingTradeStatement;
            }

            public SquaredStatement SquaredStatement { get; set; }
            public TradeStatement RemainingTradeStatement { get; set; }
        }
    }
}