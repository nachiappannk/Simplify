using Simplify.Books;

namespace Simplify.DataGateway
{
    public interface IBalanceSheetReader
    {
        BalanceSheetBook GetBalanceSheet(string previousPeriodFile);
    }
}