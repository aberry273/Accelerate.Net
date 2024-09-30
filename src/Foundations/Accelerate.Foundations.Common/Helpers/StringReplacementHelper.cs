using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Helpers
{
    public static class StringReplacementHelper
    {
        public static string ReplaceTokens(string input, IDictionary<string, string> tokens)
        {
            for (var j = 0; j < tokens?.Count(); j++)
            {
                var kv = tokens.ElementAt(j);
                input = input.Replace(kv.Key, kv.Value);
            }
            return input;
        }
        public static string ReplaceTokens(string input, IDictionary<string, object> tokens)
        {
            for (var j = 0; j < tokens?.Count(); j++)
            {
                var kv = tokens.ElementAt(j);
                var value = kv.Value.GetType() == typeof(String) ? kv.Value.ToString() : JsonConvert.SerializeObject(kv.Value);
                input = input.Replace(kv.Key, value);
            }
            return input;
        }
    }
}
