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
            var writer = new ProcessedTradeStatementsExcelGateway();
            writer.Write(outputExcelFile, processedTradeStatementsContainer);
            
            
        }
    }

    
}
