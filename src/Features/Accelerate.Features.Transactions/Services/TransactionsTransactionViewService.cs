using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Services;
using Elastic.Clients.Elasticsearch;
using System.Collections.Generic;
using Accelerate.Foundations.Portal.Services;
using Accelerate.Features.Transactions.Services;
using Accelerate.Foundations.Transactions.Models.Entities;
using Accelerate.Features.Transactions.Models.Views;
using Accelerate.Foundations.Integrations.AlphaVantage.Services;

namespace Accelerate.Features.Admin.Services
{
    public class TransactionsTransactionViewService : TransactionsBaseEntityViewService<TransactionsTransactionEntity>
    {
        private IAlphaVantageService _alphaVantageService;
        public TransactionsTransactionViewService(
            IMetaContentService metaContent,
            IPortalContentService portalContentService,
            IAlphaVantageService alphaVantageService)
            : base(metaContent, portalContentService)
        {
            _alphaVantageService = alphaVantageService;
            EntityName = "Transaction";
        }

        public override string GetEntityName(TransactionsTransactionEntity item)
        {
            return $"{item?.OnBehalfOf}";
        }
        public override async Task<TransactionsBasePage<TransactionsTransactionEntity>> CreateEntityPage(UsersUser user, TransactionsTransactionEntity item)
        {
            var model = await base.CreateEntityPage(user, item);
            var viewModel = new TransactionsTransactionPage(model);

            viewModel.RegisteredAddress = base.CreateAddressForm(user, item); 
            
            return viewModel;
        }
        public override async Task<TransactionsBasePage<TransactionsTransactionEntity>> CreateListPage(UsersUser user, IEnumerable<TransactionsTransactionEntity> item)
        {
            var model = await base.CreateListPage(user, item);
            var viewModel = new TransactionsTransactionPage(model);
            //var response = await _alphaVantageService.CurrencyExchangeRate("EUR", "USD");
            //viewModel.RealTimeExchangerate = response;
            return viewModel;
        }
    }
}
