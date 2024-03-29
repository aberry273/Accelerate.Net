using Accelerate.Features.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch.Ingest;

namespace Accelerate.Features.Content.Pipelines
{
    public class AccountUserUpdatedPipeline : DataUpdateEventPipeline<AccountUser>
    {
        IElasticService<AccountUserDocument> _elasticService;
        public AccountUserUpdatedPipeline(
            IElasticService<AccountUserDocument> elasticService)
        {
            _elasticService = elasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<AccountUser>>()
            {
                UpdateDocument,
            };
        }
        // ASYNC PROCESSORS
        public async Task UpdateDocument(IPipelineArgs<AccountUser> args)
        {
            var userId = args.Value.Id.ToString();
            //var user = await _userManager.FindByIdAsync(userId);
            var indexModel = new AccountUserDocument()
            {
                Domain = args.Value.Domain,
                Id = args.Value.Id.ToString(),
                Username = args.Value.UserName
            };
            await _elasticService.UpdateDocument<AccountUserDocument>(indexModel, args.Value.Id.ToString());
        }
    }
}
