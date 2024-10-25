
using Accelerate.Features.Account.Models;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
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
    public class AccountUserDeletedPipeline : DataDeleteEventPipeline<AccountUser>
    {
        IElasticService<AccountUserDocument> _elasticService;
        UserManager<AccountUser> _userManager;
        IEntityService<AccountProfile> _profileService;
        public AccountUserDeletedPipeline(
            UserManager<AccountUser> userManager,
            SignInManager<AccountUser> signInManager,
            IEntityService<AccountProfile> profileService,
            IElasticService<AccountUserDocument> elasticService)
        {
            _userManager = userManager;
            _profileService = profileService;
            _elasticService = elasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<AccountUser>>()
            {
                //DeleteProfile,
                DeleteDocument,
                //ScrubUserData
            };
        } 
                
        public async Task ScrubUserData(IPipelineArgs<AccountUser> args)
        {
            var user = args.Value;
            var deactivatedUsername = args.Value.Id.ToString();
            user.Status = AccountUserStatus.Deactivated;
            user.Email = deactivatedUsername+"@inactive.parot.app";
            user.UserName = deactivatedUsername;
            user.UpdatedOn = DateTime.Now;
            user.AccountProfileId = Guid.Empty;
            await _userManager.UpdateAsync(user);
        }
        public async Task DeleteProfile(IPipelineArgs<AccountUser> args)
        {
            var profile = _profileService.Get(args.Value.AccountProfileId);
            if (profile != null)
            {
                await _profileService.Delete(profile);
            }
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<AccountUser> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<AccountUserDocument>(args.Value.Id.ToString());
        }
    }
}
