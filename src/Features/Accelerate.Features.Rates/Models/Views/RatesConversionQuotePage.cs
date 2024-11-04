using Accelerate.Foundations.Rates.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Integrations.AlphaVantage.Models.Data;

namespace Accelerate.Features.Rates.Models.Views
{
    public class RateConversionQuotePage : RatesBasePage<RatesConversionQuoteEntity>
    {
        public RateConversionQuotePage(RatesBasePage<RatesConversionQuoteEntity> model) : base(model)
        {
        }
        public AjaxForm? RegisteredAddress { get; set; }
        public AlphaVantageRealtimeExchangeRate RealTimeExchangerate { get; set; }
    }
}
