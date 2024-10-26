using Accelerate.Features.Account.Models;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch.Ingest;

namespace Accelerate.Features.Content.Pipelines
{
    public class UsersUserUpdatedPipeline : DataUpdateEventPipeline<UsersUser>
    {
        IElasticService<UsersUserDocument> _elasticService;
        IEntityService<UsersProfile> _profileService;
        public UsersUserUpdatedPipeline(
            IEntityService<UsersProfile> profileService,
            IElasticService<UsersUserDocument> elasticService)
        {
            _profileService = profileService;
            _elasticService = elasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<UsersUser>>()
            {
                UpdateDocument,
            };
        }
        // ASYNC PROCESSORS
        public async Task UpdateDocument(IPipelineArgs<UsersUser> args)
        {
            var profile = _profileService.Get(args.Value.UsersProfileId.GetValueOrDefault());

            //var user = await _userManager.FindByIdAsync(userId);
            var indexModel = new UsersUserDocument()
            {
                Id = args.Value.Id,
                CreatedOn = args.Value.CreatedOn,
                UpdatedOn = args.Value.UpdatedOn,
                Domain = args.Value.Domain,
                Username = args.Value.UserName,

                Image = profile.Image,
                Firstname = profile.Firstname,
                Lastname = profile.Lastname,
            };
            await _elasticService.UpdateDocument<UsersUserDocument>(indexModel, args.Value.Id.ToString());
        }
    }
}
