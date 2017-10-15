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
        public void GenerateStatements(string tradeLogFileName, string tradeLogSheetName,
            string outputExcelFile, ILogger logger)
        {
            ProcessedTradeStatementsExcelGateway gateway = new ProcessedTradeStatementsExcelGateway();
            var tradeLogs = gateway.ReadTradeLog(logger, tradeLogFileName, tradeLogSheetName);
            
            ProcessedTradeStatementsContainer processedTradeStatementsContainer = new ProcessedTradeStatementsContainer(tradeLogs);
            var writer = new ProcessedTradeStatementsExcelGateway();

            if (File.Exists(outputExcelFile)) File.Delete(outputExcelFile);
            writer.Write(outputExcelFile, processedTradeStatementsContainer);
        }
    }
}
