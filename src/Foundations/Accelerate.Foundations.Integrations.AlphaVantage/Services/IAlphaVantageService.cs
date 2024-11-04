using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Integrations.AlphaVantage.Models.Data;

namespace Accelerate.Foundations.Integrations.AlphaVantage.Services
{
    public interface IAlphaVantageService
    {
        Task<AlphaVantageRealtimeExchangeRate> CurrencyExchangeRate(string fromCurrency, string toCurrency, string interval = "60min", string outputSize = "compact");
    }
}
