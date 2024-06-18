using Accelerate.Features.Account.Models;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch.Ingest;

namespace Accelerate.Features.Content.Pipelines
{
    public class AccountUserUpdatedPipeline : DataUpdateEventPipeline<AccountUser>
    {
        IElasticService<AccountUserDocument> _elasticService;
        IEntityService<AccountProfile> _profileService;
        public AccountUserUpdatedPipeline(
            IEntityService<AccountProfile> profileService,
            IElasticService<AccountUserDocument> elasticService)
        {
            _profileService = profileService;
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
            var profile = _profileService.Get(args.Value.AccountProfileId);

            var userId = args.Value.Id.ToString();
            //var user = await _userManager.FindByIdAsync(userId);
            var indexModel = new AccountUserDocument()
            {
                CreatedOn = args.Value.CreatedOn,
                UpdatedOn = args.Value.UpdatedOn,
                Domain = args.Value.Domain,
                Id = args.Value.Id,
                Username = args.Value.UserName,

                Image = profile.Image,
                Firstname = profile.Firstname,
                Lastname = profile.Lastname,
            };
            await _elasticService.UpdateDocument<AccountUserDocument>(indexModel, args.Value.Id.ToString());
        }
    }
}
