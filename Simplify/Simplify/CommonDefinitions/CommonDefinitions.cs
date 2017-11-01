using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplify.CommonDefinitions
{
    public static class CommonDefinitions
    {
        public static readonly double? DoubleNull = null;

        public static bool IsDoubleEqual(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) < 0.0001;
        }

        public static bool IsNullableDoubleEqual(this double? value1, double? value2)
        {
            if ((value1 == null) && (value2 == null)) return true;
            if (value1.HasValue && value2.HasValue)
                return IsDoubleEqual(value1.Value, value2.Value);
            return false;
        }
    }
}
