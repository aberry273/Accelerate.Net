using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.AlphaVantage.Models.Data
{
    public class AlphaVantageTimeseries
    {
        [JsonProperty("Meta Data")]
        public AlphaVantageMetaData MetaData { get; set; }
        [JsonProperty("Time Series (Daily)")]
        public Dictionary<string, TimeSeriesEntry> TimeSeries { get; set; }
        [JsonProperty("Time Series (60min)")]
        private Dictionary<string, TimeSeriesEntry> TimeSeries60min { set { TimeSeries = value; } }
    }
    
    public class TimeSeriesEntry
    {
        [JsonProperty("1. open")]
        public string Open { get; set; }
        [JsonProperty("2. high")]
        public string High { get; set; }
        [JsonProperty("3. low")]
        public string Low { get; set; }
        [JsonProperty("4. close")]
        public string Close { get; set; }
        [JsonProperty("5. adjusted close")]
        public string AdjustedClose { get; set; }
        [JsonProperty("6. volume")]
        public string Volume { get; set; }
        [JsonProperty("7. dividend amount")]
        public string DividendAmount { get; set; }
        [JsonProperty("8. split coefficient")]
        public string SplitCoefficient { get; set; }
    }
}
