using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, Task<TResult>> method)
        {
            return await Task.WhenAll(source.Select(async s => await method(s)));
        }
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
