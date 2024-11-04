using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.AlphaVantage.Models.Api
{ 
    public class CurrencyExchangeRateResponse
    {
        [JsonPropertyName("Meta Data")]
        public dynamic MetaData { get; set; }

        [JsonPropertyName("Time Series (60min)")]
        public List<KeyValuePair<string, string>> TimeSeriesFX { get; set; }
    }
}
