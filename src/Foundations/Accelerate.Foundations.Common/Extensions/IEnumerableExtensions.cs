using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool ContainIgnoreCase(this IEnumerable<string> list, string value)
        {
            if (list == null || !list.Any())
                return false;

            if (value == null)
                return false;

            return list.Any(item => item.Equals(value, StringComparison.OrdinalIgnoreCase));
        }
    }
}
