using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplify.Books
{
    public class TradeStatement
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }
        public string[] OtherDetails { get; set; }
    }

    public class CompletedStatement
    {
        public TradeStatement PurchaseTradeStatement { get; set; }
        public TradeStatement SaleTradeStatement { get; set; }
    }


    public class CapitalGainsStatementComputer
    {
        public List<CompletedStatement> CompetedStatements { get; private set; }
        public List<TradeStatement> OpenTradeStatements { get; private set; }
        public void Compute(IList<TradeStatement> tradeStatements)
        {
            CompetedStatements = new List<CompletedStatement>();
            OpenTradeStatements = new List<TradeStatement>();
            var names = tradeStatements.Select(x => x.Name).Distinct();
            foreach (var name in names)
            {
                GetTrade(tradeStatements);
            }
            
        }
        

        private void GetTrade(IList<TradeStatement> tradeStatements)
        {
            var purchaseStatements = tradeStatements.Where(x => x.Quantity > 0).OrderBy(x => x.Date).ToList();
            var saleStatements = tradeStatements.Where(x => x.Quantity > 0).OrderBy(x => x.Date).ToList();
            while(purchaseStatements.Count > 0 && saleStatements.Count > 0)
            { 
                TradeStatement trade = PushCompletedAndGiveRemainingIfAvailable(
                    RemoveAndProvideFirstItem(purchaseStatements),
                    RemoveAndProvideFirstItem(saleStatements));
                if (trade == null) continue;
                if (trade.Quantity > 0.001)
                    purchaseStatements.Insert(0, trade);
                else
                    saleStatements.Insert(0, trade);
            }
            if (purchaseStatements.Count > 0)
                OpenTradeStatements.AddRange(purchaseStatements);
            if (saleStatements.Count > 0)
                OpenTradeStatements.AddRange(saleStatements);
        }

        private TradeStatement RemoveAndProvideFirstItem(IList<TradeStatement> statements)
        {
            var statement = statements.ElementAt(0);
            return statement;
        }

        private TradeStatement PushCompletedAndGiveRemainingIfAvailable(TradeStatement tradeStatement, TradeStatement tradeStatement1)
        {
            throw new NotImplementedException();
        }

        private void AddToCompletedStatements(params TradeStatement[] statements)
        {
            var completedStatement = new CompletedStatement();
            foreach (var statement in statements)
            {
                if (statement.Quantity > 0)
                    completedStatement.PurchaseTradeStatement = statement;
                else
                    completedStatement.SaleTradeStatement = statement;
            }
            CompetedStatements.Add(completedStatement);
        }
    }
}
