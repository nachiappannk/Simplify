using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplify.Application;
using Simplify.Books;
using Simplify.ExcelDataGateway;

namespace Simplify.Facade
{
    public class CapitalGainsStatementGenerationFacade
    {
        public void GenerateStatements(string openingStockFileName, 
            string openingStockSheetName, string tradeLogFileName, string tradeLogSheetName,
            string outputExcelFile, ILogger logger)
        {
            TradeLogGateway openStockGateway = new TradeLogGateway(openingStockFileName);
            var openingStock = openStockGateway.ReadTradeLog(logger, openingStockSheetName);

            if (File.Exists(outputExcelFile)) File.Delete(outputExcelFile);
            TradeLogGateway tradeLogGateway = new TradeLogGateway(tradeLogFileName);
            var tradeLogs = tradeLogGateway.ReadTradeLog(logger, tradeLogSheetName);
            tradeLogs.AddRange(openingStock);

            ProcessedTradeStatementsContainer processedTradeStatementsContainer = new ProcessedTradeStatementsContainer(tradeLogs);
            var writeLogGateway = new TradeLogGateway(outputExcelFile);
            var openPositions = processedTradeStatementsContainer.OpenPositionBook.Select(x => new SquarableStatement(x)).ToList();
            var namedBooks = processedTradeStatementsContainer.AssetSummaryBooks;
            List<SquarableStatement> summaryStatements = new List<SquarableStatement>();
            foreach (var namedBook in namedBooks)
            {
                foreach (var statments in namedBook.Values)
                {
                    summaryStatements.AddRange(statments);
                }
                
            }
            writeLogGateway.WriteSummary(summaryStatements);
            writeLogGateway.WriteOpenPositions(openPositions);

            var capitalGainsStatementWriter = new CapitalGainsStatementWriter(outputExcelFile);
            capitalGainsStatementWriter.WriteCapitalGains(processedTradeStatementsContainer.ProfitBook);
        }
    }
}
