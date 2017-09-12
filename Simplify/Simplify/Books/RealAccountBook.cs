using System.Collections.Generic;

namespace Simplify.Books
{
    public class RealAccountBook : List<DatedStatement>
    {

        public RealAccountBook(string accountName)
        {
            AccountName = accountName;
        }

        public string AccountName { get; private set; }
    }
}