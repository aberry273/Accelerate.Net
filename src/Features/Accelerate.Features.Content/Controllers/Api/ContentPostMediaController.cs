﻿using Accelerate.Foundations.Content.EventBus;
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
    public class ContentPostMediaController : BaseApiController<ContentPostMediaEntity>
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        readonly Bind<IContentPostActivityBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<ContentPostDocument> _searchService;
        IEntityService<ContentPostEntity> _postService;
        public ContentPostMediaController(
            IMetaContentService contentService,
            IEntityService<ContentPostMediaEntity> service,
            IEntityService<ContentPostEntity> postService,
            Bind<IContentPostActivityBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostDocument> searchService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
            _postService = postService;
        }


        protected override async Task PostCreateSteps(ContentPostMediaEntity obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<ContentPostMediaEntity>() { Data = obj });
        }
        protected override void UpdateValues(ContentPostMediaEntity from, dynamic to)
        {
            from.MediaBlobId = to.MediaBlobId;
        }
        protected override async Task PostUpdateSteps(ContentPostMediaEntity obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<ContentPostMediaEntity>() { Data = obj });
        }
        protected override async Task PostDeleteSteps(ContentPostMediaEntity obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<ContentPostMediaEntity>() { Data = obj });
        }
    }
}