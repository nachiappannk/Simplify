namespace Simplify.Books
{
    public class JournalStatement : DatedStatement
    {
        
        public Book BookName { get; set; }
        public string AdditionalName { get; set; }
        public string EntryType { get; set; }
    }
}