namespace Simplify.Books
{
    public enum Book
    {
        ProfitAndLoss,
        Capital,
        BalanceSheet
    }

    public static class BookExnt
    {
        public static Book GetBook(this string bookName)
        {
            switch (bookName)
            {
                case "P&L":
                    return Book.ProfitAndLoss;
                case "CAP":
                    return Book.Capital;
                case "BS":
                    return Book.BalanceSheet;
                default:
                    return Book.ProfitAndLoss;
            }
        }

        public static string GetString(this Book book)
        {
            switch (book)
            {
                case Book.ProfitAndLoss:
                    return "P&L";
                case Book.Capital:
                    return "CAP";
                case Book.BalanceSheet:
                    return "BS";
                default:
                    return "P&L";
            }
        }
    }

}