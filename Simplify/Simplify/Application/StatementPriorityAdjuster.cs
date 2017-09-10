using System;
using System.Collections.Generic;
using Simplify.Books;

namespace Simplify.Application
{
    public class StatementPriorityAdjuster
    {
        private Dictionary<StatementPriority, int> _priorityToHoursConvertor = new Dictionary<StatementPriority, int>()
        {
            {StatementPriority.Opening, 1},
            {StatementPriority.Normal, 2},
            {StatementPriority.PreClosing, 2},
            {StatementPriority.Closing, 2},
        };


        public DetailedDatedStatement RePrioritizeStatement(DetailedDatedStatement detailedDatedStatement , StatementPriority priority)
        {
            var ret = detailedDatedStatement.CreateCopy();
            var date = ret.Date;
            ret.Date = new DateTime(date.Year, date.Month, date.Day, _priorityToHoursConvertor[priority],0,0);
            return ret;
        }

        public DetailedDatedStatement ReDetailedDatedStatementAsOpening(DetailedDatedStatement detailedDatedStatement)
        {
            return RePrioritizeStatement(detailedDatedStatement, StatementPriority.Opening);
        }
        public DetailedDatedStatement ReDetailedDatedStatementAsClosing(DetailedDatedStatement detailedDatedStatement)
        {
            return RePrioritizeStatement(detailedDatedStatement, StatementPriority.Closing);
        }
        public DetailedDatedStatement ReDetailedDatedStatementAsPreClosing(DetailedDatedStatement detailedDatedStatement)
        {
            return RePrioritizeStatement(detailedDatedStatement, StatementPriority.PreClosing);
        }
        public DetailedDatedStatement ReDetailedDatedStatementAsNormal(DetailedDatedStatement detailedDatedStatement)
        {
            return RePrioritizeStatement(detailedDatedStatement, StatementPriority.Normal);
        }
    }

    public enum StatementPriority
    {
        Opening,
        Normal,
        PreClosing,
        Closing,
    }
}