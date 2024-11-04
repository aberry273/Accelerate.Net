using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.AlphaVantage.Models.Data
{
    public class AlphaVantageRealtimeExchangeRate
    {
        [JsonProperty("Realtime Currency Exchange Rate")]
        public RealtimeExchangeRateData RealtimeCurrencyExchangeRate { get; set; }
    }
    
    public class RealtimeExchangeRateData
    {
        [JsonProperty("1. From_Currency Code")]
        public string FromCurrencyCode { get; set; }
        [JsonProperty("2. From_Currency Name")]
        public string FromCurrencyName { get; set; }
        [JsonProperty("3. To_Currency Code")]
        public string ToCurrencyCode { get; set; }
        [JsonProperty("4. To_Currency Name")]
        public string ToCurrencyName { get; set; }
        [JsonProperty("5. Exchange Rate")]
        public decimal ExchangeRate { get; set; }
        [JsonProperty("6. Last Refreshed")]
        public DateTime LastRefreshed { get; set; }
        [JsonProperty("7. Time Zone")]
        public string TimeZone { get; set; }
        [JsonProperty("8. Bid Price")]
        public decimal BidPrice { get; set; }
        [JsonProperty("9. Ask Price")]
        public decimal AskPrice { get; set; }
    }
}
