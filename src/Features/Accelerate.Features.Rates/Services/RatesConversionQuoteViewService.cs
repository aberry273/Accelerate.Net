using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Services;
using Elastic.Clients.Elasticsearch;
using System.Collections.Generic;
using Accelerate.Foundations.Portal.Services;
using Accelerate.Features.Rates.Services;
using Accelerate.Foundations.Rates.Models.Entities;
using Accelerate.Features.Rates.Models.Views;
using Accelerate.Foundations.Integrations.AlphaVantage.Services;
using Accelerate.Foundations.Integrations.AlphaVantage.Models.Data;

namespace Accelerate.Features.Admin.Services
{
    public class RatesConversionQuoteViewService : RatesBaseEntityViewService<RatesConversionQuoteEntity>
    {
        private IAlphaVantageService _alphaVantageService;
        private static List<AlphaVantageRealtimeExchangeRate> DailyRateExchanges { get; set; }
        public RatesConversionQuoteViewService(
            IMetaContentService metaContent,
            IPortalContentService portalContentService,
            IAlphaVantageService alphaVantageService)
            : base(metaContent, portalContentService)
        {
            _alphaVantageService = alphaVantageService;
            EntityName = "Quote";
            DailyRateExchanges = new List<AlphaVantageRealtimeExchangeRate>();
        }

        public override string GetEntityName(RatesConversionQuoteEntity item)
        {
            return $"{item.SellAsset} (${item.SellPrice}) > {item.BuyAsset} (${item.BuyPrice})";
        }
        public override async Task<RatesBasePage<RatesConversionQuoteEntity>> CreateEntityPage(UsersUser user, RatesConversionQuoteEntity item)
        {
            var model = await base.CreateEntityPage(user, item);
            var viewModel = new RateConversionQuotePage(model);

            viewModel.RegisteredAddress = base.CreateAddressForm(user, item); 
            
            return viewModel;
        }
        public override async Task<RatesBasePage<RatesConversionQuoteEntity>> CreateListPage(UsersUser user, IEnumerable<RatesConversionQuoteEntity> item)
        {
            var model = await base.CreateListPage(user, item);
            var viewModel = new RateConversionQuotePage(model);
            var rate = DailyRateExchanges.FirstOrDefault(x => x.RealtimeCurrencyExchangeRate.FromCurrencyName == "EUR");
            if(rate == null)
            {
                var rates = await _alphaVantageService.CurrencyExchangeRate("EUR", "USD");
                DailyRateExchanges.Add(rates);
            }
            viewModel.RealTimeExchangerate = rate;
            return viewModel;
        }
    }
}
