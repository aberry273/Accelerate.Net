using Accelerate.Foundations.Content.EventBus;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Users.Models.Entities;
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
    public class ContentChannelController : BaseApiServiceController<ContentChannelEntity>
    { 
        UserManager<UsersUser> _userManager;
        IMetaContentService _contentService;
        readonly Bind<IContentChannelBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<ContentChannelDocument> _searchService;
        IEntityService<ContentPostEntity> _postService;
        public ContentChannelController(
            IMetaContentService contentService,
            IEntityService<ContentChannelEntity> service,
            IEntityService<ContentPostEntity> postService,
            Bind<IContentChannelBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentChannelDocument> searchService,
            UserManager<UsersUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
            _postService = postService;
        }


        protected override async Task PostCreateSteps(ContentChannelEntity obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<ContentChannelEntity>() { Data = obj });
        }
        protected override void UpdateValues(ContentChannelEntity from, dynamic to)
        {
            from.Tags = to.Tags;
            from.Description = to.Description;
            from.Name = to.Name;
            from.Category = to.Category;
            from.Status = to.Status;
        }
        protected override async Task PostUpdateSteps(ContentChannelEntity obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<ContentChannelEntity>() { Data = obj });
        }
        protected override async Task PostDeleteSteps(ContentChannelEntity obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<ContentChannelEntity>() { Data = obj });
        }
    }
}