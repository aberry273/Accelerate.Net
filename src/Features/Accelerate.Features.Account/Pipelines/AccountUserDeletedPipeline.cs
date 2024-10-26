
using Accelerate.Features.Account.Models;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch.Ingest;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity;

namespace Accelerate.Features.Account.Pipelines
{
    public class UsersUserDeletedPipeline : DataDeleteEventPipeline<UsersUser>
    {
        IElasticService<UsersUserDocument> _elasticService;
        UserManager<UsersUser> _userManager;
        IEntityService<UsersProfile> _profileService;
        public UsersUserDeletedPipeline(
            UserManager<UsersUser> userManager,
            SignInManager<UsersUser> signInManager,
            IEntityService<UsersProfile> profileService,
            IElasticService<UsersUserDocument> elasticService)
        {
            _userManager = userManager;
            _profileService = profileService;
            _elasticService = elasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<UsersUser>>()
            {
                //DeleteProfile,
                DeleteDocument,
                //ScrubUserData
            };
        } 
                
        public async Task ScrubUserData(IPipelineArgs<UsersUser> args)
        {
            var user = args.Value;
            var deactivatedUsername = args.Value.Id.ToString();
            user.Status = UsersUserStatus.Deactivated;
            user.Email = deactivatedUsername+"@inactive.parot.app";
            user.UserName = deactivatedUsername;
            user.UpdatedOn = DateTime.Now;
            user.UsersProfileId = Guid.Empty;
            await _userManager.UpdateAsync(user);
        }
        public async Task DeleteProfile(IPipelineArgs<UsersUser> args)
        {
            var profile = _profileService.Get(args.Value.UsersProfileId.GetValueOrDefault());
            if (profile != null)
            {
                await _profileService.Delete(profile);
            }
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<UsersUser> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<UsersUserDocument>(args.Value.Id.ToString());
        }
    }
}
