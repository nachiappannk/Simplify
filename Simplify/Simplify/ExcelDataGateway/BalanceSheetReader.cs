using System;
using Simplify.Books;
using Simplify.DataGateway;

namespace Simplify.ExcelDataGateway
{
    class BalanceSheetReader : IBalanceSheetReader
    {
        private string inputExcelFile;

        public BalanceSheetReader(string inputExcelFile)
        {
            this.inputExcelFile = inputExcelFile;
        }

        

        public BalanceSheetBook GetBalanceSheet(string previousPeriodFile)
        {
            throw new NotImplementedException();
        }
    }
}