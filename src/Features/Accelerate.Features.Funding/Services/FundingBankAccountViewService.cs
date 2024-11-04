using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Services;
using Elastic.Clients.Elasticsearch;
using System.Collections.Generic;
using Accelerate.Foundations.Portal.Services;
using Accelerate.Features.Funding.Services;
using Accelerate.Foundations.Funding.Models.Entities;
using Accelerate.Features.Funding.Models.Views;
using Accelerate.Foundations.Integrations.AlphaVantage.Services;

namespace Accelerate.Features.Admin.Services
{
    public class FundingBankAccountViewService : FundingBaseEntityViewService<FundingBankAccountEntity>
    {
        private IAlphaVantageService _alphaVantageService;
        public FundingBankAccountViewService(
            IMetaContentService metaContent,
            IPortalContentService portalContentService,
            IAlphaVantageService alphaVantageService)
            : base(metaContent, portalContentService)
        {
            _alphaVantageService = alphaVantageService;
            EntityName = "Quote";
        }

        public override string GetEntityName(FundingBankAccountEntity item)
        {
            return $"{item.AccountNumber}:{item.BankName}";
        }
        public override async Task<FundingBasePage<FundingBankAccountEntity>> CreateEntityPage(UsersUser user, FundingBankAccountEntity item)
        {
            var model = await base.CreateEntityPage(user, item);
            var viewModel = new FundingBankAccountPage(model);

            viewModel.RegisteredAddress = base.CreateAddressForm(user, item); 
            
            return viewModel;
        }
        public override async Task<FundingBasePage<FundingBankAccountEntity>> CreateListPage(UsersUser user, IEnumerable<FundingBankAccountEntity> item)
        {
            var model = await base.CreateListPage(user, item);
            var viewModel = new FundingBankAccountPage(model);
            //var response = await _alphaVantageService.CurrencyExchangeRate("EUR", "USD");
            //viewModel.RealTimeExchangerate = response;
            return viewModel;
        }
    }
}
