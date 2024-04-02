﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Helpers
{
    public static class JsonSerializerHelper
    {
        public static string SerializeObject(object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new SubstituteNullWithEmptyStringContractResolver(),
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T DeserializeObject<T>(string obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new SubstituteNullWithEmptyStringContractResolver(),
                Formatting = Formatting.Indented
            };
            return JsonConvert.DeserializeObject<T>(obj, settings);
        }
    }
}