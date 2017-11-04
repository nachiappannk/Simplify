using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifyUi.Common
{
    public static class ToStringExtentionMethods
    {
        public static string ToStringDisplayable(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        
    }
}
