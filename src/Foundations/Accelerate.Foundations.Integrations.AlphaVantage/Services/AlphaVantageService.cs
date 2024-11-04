using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using System.Text;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Integrations.AlphaVantage.Models;
using System.IO;
using Accelerate.Foundations.Integrations.AlphaVantage.Models.Api;
using Accelerate.Foundations.Integrations.AlphaVantage.Models.Data;

namespace Accelerate.Foundations.Integrations.AlphaVantage.Services
{
    public class AlphaVantageService : IAlphaVantageService
    {
        private readonly string baseUrl = "https://www.alphavantage.co";
        AlphaVantageConfiguration _config;
        IResilientHttpClient _resilientHttpClient;

        public AlphaVantageService(IOptions<AlphaVantageConfiguration> options, IResilientHttpClient resilientHttpClient)
        {
            _resilientHttpClient = resilientHttpClient;
            _config = options.Value;
            _resilientHttpClient.InitializeClient(baseUrl);
        }

        public async Task<AlphaVantageRealtimeExchangeRate> CurrencyExchangeRate(string fromCurrency, string toCurrency, string interval = "60min", string outputSize = "compact")
        {
            var path = $"query?function=CURRENCY_EXCHANGE_RATE&from_currency={fromCurrency}&to_currency={toCurrency}&apikey={_config.ApiKey}&interval={interval}&outputSize={outputSize}";
            var req = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/{path}");
           
            using (var resp = await _resilientHttpClient.SendAsync(req))
            {
                var body = await resp.Content.ReadAsStringAsync();
                return Foundations.Common.Helpers.JsonSerializerHelper.DeserializeObject<AlphaVantageRealtimeExchangeRate>(body);

                //return Newtonsoft.Json.Linq.JObject.Parse(body);
            }
        }
    }
}
