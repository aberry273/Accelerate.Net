using Accelerate.Features.Account.Models;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.Identity;

namespace Accelerate.Features.Account.Pipelines
{
    public class UsersUserCreatedPipeline : DataCreateEventPipeline<UsersUser>
    {
        IElasticService<UsersUserDocument> _elasticService;
        public UsersUserCreatedPipeline(
            IElasticService<UsersUserDocument> elasticService)
        {
            _elasticService = elasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<UsersUser>>()
            {
                IndexDocument
            };
            _processors = new List<PipelineProcessor<UsersUser>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<UsersUser> args)
        {
            var userId = args.Value.Id.ToString();
            //var user = await _userManager.FindByIdAsync(userId);
            var indexModel = new UsersUserDocument()
            {
                CreatedOn = args.Value.CreatedOn,
                UpdatedOn = args.Value.UpdatedOn,
                Domain = args.Value.Domain,
                Id = args.Value.Id,
                Username = args.Value.UserName,
                Firstname = args.Value?.UsersProfile?.Firstname,
                Lastname = args.Value?.UsersProfile?.Lastname,
                Image = args.Value?.UsersProfile?.Image,
            };
            await _elasticService.Index(indexModel);
        }
        // SYNC PROCESSORS
    }
}
