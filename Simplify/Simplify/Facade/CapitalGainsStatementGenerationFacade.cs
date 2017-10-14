using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplify.Application;
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

            SquaredAndOpenTradeSeparator squaredAndOpenTradeSeparator = new SquaredAndOpenTradeSeparator();
            var result = squaredAndOpenTradeSeparator.Separate(tradeLogs);
                var writeLogGateway = new TradeLogGateway(outputExcelFile);
            var openPositions = result.Where(x => !x.IsDealClosed).ToList();
                writeLogGateway.WriteSummary(result);
                writeLogGateway.WriteOpenPositions(openPositions);

                var capitalGainsStatementWriter = new CapitalGainsStatementWriter(outputExcelFile);
            var closedPositions = result.Where(x => x.IsDealClosed).ToList();
            capitalGainsStatementWriter.WriteCapitalGains(closedPositions);
        }
    }
}
