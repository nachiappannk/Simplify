namespace Simplify.Books
{
    public class NotionalAccount
    {
        public string RealAccountName { get; set; }
        public string NotionalAccountName { get; set; }

        public override bool Equals(object obj)
        {
            var refObj = obj as NotionalAccount;
            if (refObj == null) return false;
            if (refObj.RealAccountName != RealAccountName) return false;
            if (refObj.NotionalAccountName != NotionalAccountName) return false;
            return true;
        }
        
        public override int GetHashCode()
        {
            return 1;
        }
    }
}