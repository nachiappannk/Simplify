using System.Collections.Generic;
using Simplify.Books;

namespace Simplify.DataGateway
{
    public interface IJournalReader
    {
        IList<JournalStatement> GetJournal(string inputFile);
    }
}