
using Accelerate.Features.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch.Ingest;

namespace Accelerate.Features.Account.Pipelines
{
    public class AccountUserDeletedPipeline : DataDeleteEventPipeline<AccountUser>
    {
        IElasticService<AccountUserDocument> _elasticService;
        public AccountUserDeletedPipeline(
            IElasticService<AccountUserDocument> elasticService)
        {
            _elasticService = elasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<AccountUser>>()
            {
                DeleteDocument,
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<AccountUser> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<AccountUserDocument>(args.Value.Id.ToString());
        }
    }
}
