using Simplify.Books;

namespace Simplify.DataGateway
{
    public interface IBooksOfAccountWriter
    {
        void WriteBooksOfAccount(ConsolidatedBook consolidatedBook);
    }
}