using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplify.Application;
using Simplify.ExcelDataGateway;

namespace Simplify.Facade
{
    public class CapitalGainsStatementGenerationFacade
    {
        public void GenerateStatements(string inputExcelFileName, 
            string excelSheetName, 
            string outputExcelFile, ILogger logger)
        {

            try
            {
                TradeLogGateway tradeLogGateway = new TradeLogGateway(inputExcelFileName);
                var tradeLogs = tradeLogGateway.ReadTradeLog(logger, excelSheetName);

                SquaredAndOpenTradeSeparator squaredAndOpenTradeSeparator = new SquaredAndOpenTradeSeparator();
                var result = squaredAndOpenTradeSeparator.Separate(tradeLogs);
                    var writeLogGateway = new TradeLogGateway(outputExcelFile);
                    writeLogGateway.WriteOpenPositions(result.OpenTradeStatements);

                    var capitalGainsStatementWriter = new CapitalGainsStatementWriter(outputExcelFile);
                    capitalGainsStatementWriter.WriteCapitalGains(result.SquaredStatements);
            }
            catch (Exception e)
            {
                
            }
            
        }
    }
}
