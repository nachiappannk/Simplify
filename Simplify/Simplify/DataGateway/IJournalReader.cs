using System.Collections.Generic;
using Simplify.Books;

namespace Simplify.DataGateway
{
    public interface IJournalReader : IBalanceSheetReader
    {
        IList<JournalStatement> GetJournal(string inputFile);
    }
}