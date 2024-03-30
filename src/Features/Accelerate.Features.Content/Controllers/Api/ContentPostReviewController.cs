using Accelerate.Features.Content.EventBus;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Content.Models.Data;
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
    public class ContentPostReviewController : BaseApiController<ContentPostReviewEntity>
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        readonly Bind<IContentBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<ContentPostDocument> _searchService;
        IEntityService<ContentPostEntity> _postService;
        public ContentPostReviewController(
            IMetaContentService contentService,
            IEntityService<ContentPostReviewEntity> service,
            IEntityService<ContentPostEntity> postService,
            Bind<IContentBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostDocument> searchService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
            _postService = postService;
        }


        protected override async Task PostCreateSteps(ContentPostReviewEntity obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<ContentPostReviewEntity>() { Data = obj });
        }
        protected override void UpdateValues(ContentPostReviewEntity from, dynamic to)
        {
            from.Like = to.Like;
            from.Disagree = to.Disagree;
            from.Agree = to.Agree;
        }
        protected override async Task PostUpdateSteps(ContentPostReviewEntity obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<ContentPostReviewEntity>() { Data = obj });
        }
        protected override async Task PostDeleteSteps(ContentPostReviewEntity obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<ContentPostReviewEntity>() { Data = obj });
        }
    }
}