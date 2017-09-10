using System.Text.RegularExpressions;
using Simplify.Books;

namespace Simplify.Application
{
    public class JournalStatementBracketTrimmer
    {
        public DetailedDatedStatement Trim(DetailedDatedStatement detailedDatedStatement)
        {
            var trimmedName = Regex.Replace(detailedDatedStatement.Description, "\\([a-zA-Z0-9\\s]*\\)", string.Empty).Trim();
            var retVal = detailedDatedStatement.CreateCopy();
            retVal.Description = trimmedName;
            return retVal;
        }
    }
}