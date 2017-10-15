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
            ProcessedTradeStatementsExcelGateway gateway = new ProcessedTradeStatementsExcelGateway();
            var openingStock = gateway.ReadTradeLog(logger, openingStockFileName, openingStockSheetName);

            ProcessedTradeStatementsExcelGateway gateway1 = new ProcessedTradeStatementsExcelGateway();
            var tradeLogs = gateway1.ReadTradeLog(logger, openingStockFileName, openingStockSheetName);

            tradeLogs.AddRange(openingStock);


            ProcessedTradeStatementsContainer processedTradeStatementsContainer = new ProcessedTradeStatementsContainer(tradeLogs);
            var writer = new ProcessedTradeStatementsExcelGateway();

            if (File.Exists(outputExcelFile)) File.Delete(outputExcelFile);
            writer.Write(outputExcelFile, processedTradeStatementsContainer);
        }
    }
}
