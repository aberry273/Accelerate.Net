using Accelerate.Foundations.Content.EventBus;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Integrations.Contracts;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using MassTransit;
using MassTransit.DependencyInjection;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentFeedController : BaseApiController<ContentFeedEntity>
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        readonly Bind<IContentFeedBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<ContentFeedDocument> _searchService;
        IEntityService<ContentPostEntity> _postService;
        public ContentFeedController(
            IMetaContentService contentService,
            IEntityService<ContentFeedEntity> service,
            IEntityService<ContentPostEntity> postService,
            Bind<IContentFeedBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentFeedDocument> searchService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
            _postService = postService;
        }


        protected override async Task PostCreateSteps(ContentFeedEntity obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<ContentFeedEntity>() { Data = obj });
        }
        protected override void UpdateValues(ContentFeedEntity from, dynamic to)
        {
            from.Tags = to.Tags;
            from.Description = to.Description;
            from.Name = to.Name;
            from.Category = to.Category;
            from.Status = to.Status;
        }
        protected override async Task PostUpdateSteps(ContentFeedEntity obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<ContentFeedEntity>() { Data = obj });
        }
        protected override async Task PostDeleteSteps(ContentFeedEntity obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<ContentFeedEntity>() { Data = obj });
        }
    }
}