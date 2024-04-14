using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            return $"{char.ToUpper(input[0])}{input[1..]}";
        }
        public static string ToCamelCase(this string str)
        {
            if (str == null || str.Length == 0) return string.Empty;
            return Char.ToLowerInvariant(str[0]) + str.Substring(1);

        }
    }
}
