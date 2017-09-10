using System;
using System.Collections.Generic;
using System.Linq;
using Simplify.Books;

namespace Simplify.Application
{
    public class NotionalBooksGenerator
    {
        private class SeperatedDescription
        {
            public string NotionalAccount { get; set; }
            public string RealAccount { get; set; }
            public string Tag { get; set; }
        }

        private readonly Dictionary<NotionalAccount, NotionalAccountBook> _notionalAccountBooksLookUp 
            = new Dictionary<NotionalAccount, NotionalAccountBook>();

        public bool IsNotionalAccountStatement(DetailedDatedStatement statement)
        {
            return statement.Description.Contains(@"\");
        }

        public void AddNotionalAccountStatement(DetailedDatedStatement statement)
        {
            if(!IsNotionalAccountStatement(statement)) throw new SystemException();
            var seperatedDescription = SplitDescription(statement.Description);
            var notionalAccount = new NotionalAccount()
            {
                RealAccountName = seperatedDescription.RealAccount,
                NotionalAccountName = seperatedDescription.NotionalAccount,
            };
            var detailedDescription = statement.CreateCopy();
            detailedDescription.Description = seperatedDescription.Tag;
            detailedDescription.DetailedDescription = statement.DetailedDescription; 

            AddStatement(notionalAccount, detailedDescription);
        }

        public List<NotionalAccountBook> GetNotionalAccountBooks()
        {
            return _notionalAccountBooksLookUp.Select(x => x.Value).ToList();
        }

        private void AddStatement(NotionalAccount notionalAccount,
            DetailedDatedStatement detailedDatedStatement)
        {
            if (!_notionalAccountBooksLookUp.ContainsKey(notionalAccount))
                _notionalAccountBooksLookUp.Add(notionalAccount, new NotionalAccountBook(notionalAccount));
            _notionalAccountBooksLookUp[notionalAccount].Add(detailedDatedStatement);
        }

        private SeperatedDescription SplitDescription(string journalEntryName)
        {
            var names = journalEntryName.Split(new[] {'\\'}, 3);
            if(names.Length <2) throw new SystemException();
            if(string.IsNullOrWhiteSpace(names[0])) throw new SystemException();
            if(string.IsNullOrWhiteSpace(names[1])) throw new SystemException();
            var tag = string.Empty;
            if (names.Length > 2 && !string.IsNullOrWhiteSpace(names[2])) tag = names[2];
            return new SeperatedDescription()
            {
                RealAccount = names[0].Trim(),
                NotionalAccount = names[1].Trim(),
                Tag = tag,
            };
        }
    }
}