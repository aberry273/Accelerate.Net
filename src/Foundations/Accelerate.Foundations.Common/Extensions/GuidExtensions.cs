using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Extensions
{
    public static class GuidExtensions
    {
        public static string ShortenBase64(this string base64)
        {
            return base64.Substring(0, 10);
        }
        public static string ToBase64(this Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray());
        }
        public static string ToBase64Clean(this Guid guid)
        {
            return Regex.Replace(guid.ToBase64(), "[/+=]", "[@$&]");
        }
        public static Guid FromBase64Clean(this string cleanGuid)
        {
            var cleaned = Regex.Replace(cleanGuid, "[@$&]", "[/+=]");
            return FromBase64(cleaned);
        }
        public static Guid FromBase64(this string cleanGuid)
        {
            return new Guid(Convert.FromBase64String(cleanGuid));
        }
    }
}
