using System.Text.RegularExpressions;
using Simplify.Books;

namespace Simplify.Application
{
    public class JournalStatementBracketTrimmer
    {
        public string Trim(string name)
        {
            return Regex.Replace(name, "\\([a-zA-Z0-9\\s]*\\)", string.Empty).Trim();
           
        }
    }
}