namespace Simplify.Books
{
    public class DetailedDatedStatement : DatedStatement
    {
        public string DetailedDescription { get; set; }
    }

    public static class DatedDescriptiveStatementExtention
    {
        public static DetailedDatedStatement CreateCopy(this DetailedDatedStatement detailedDatedStatement)
        {
            return new DetailedDatedStatement()
            {
                DetailedDescription = detailedDatedStatement.DetailedDescription,
                Date = detailedDatedStatement.Date,
                Description = detailedDatedStatement.Description,
                Value = detailedDatedStatement.Value,

            };
        }
    }
}