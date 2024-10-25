using Accelerate.Features.Account.Models;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.Identity;

namespace Accelerate.Features.Account.Pipelines
{
    public class AccountUserCreatedPipeline : DataCreateEventPipeline<AccountUser>
    {
        IElasticService<AccountUserDocument> _elasticService;
        public AccountUserCreatedPipeline(
            IElasticService<AccountUserDocument> elasticService)
        {
            _elasticService = elasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<AccountUser>>()
            {
                IndexDocument
            };
            _processors = new List<PipelineProcessor<AccountUser>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<AccountUser> args)
        {
            var userId = args.Value.Id.ToString();
            //var user = await _userManager.FindByIdAsync(userId);
            var indexModel = new AccountUserDocument()
            {
                CreatedOn = args.Value.CreatedOn,
                UpdatedOn = args.Value.UpdatedOn,
                Domain = args.Value.Domain,
                Id = args.Value.Id,
                Username = args.Value.UserName,
                Firstname = args.Value?.AccountProfile?.Firstname,
                Lastname = args.Value?.AccountProfile?.Lastname,
                Image = args.Value?.AccountProfile?.Image,
            };
            await _elasticService.Index(indexModel);
        }
        // SYNC PROCESSORS
    }
}
