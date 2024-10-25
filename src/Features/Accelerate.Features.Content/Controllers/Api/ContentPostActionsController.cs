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
using static Elastic.Clients.Elasticsearch.JoinField;
using Accelerate.Foundations.Content.Services;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentPostActionsController : BaseApiController<ContentPostActionsEntity>
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        readonly Bind<IContentActionsBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<ContentPostDocument> _searchService;
        IContentPostService _postService;
        public ContentPostActionsController(
            IMetaContentService contentService,
            IEntityService<ContentPostActionsEntity> service,
            IContentPostService postService,
            Bind<IContentActionsBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostDocument> searchService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
            _postService = postService;
        }

        [Route("agree")]
        [HttpPost]
        public async Task<IActionResult> Agree([FromBody] ContentPostActionsEntity obj)
        {
            var existingAction = _service.Find(x => x.Id == obj.Id).FirstOrDefault();
            obj.Agree = true;
            obj.Disagree = false;
            if (existingAction != null)
            {
                this.UpdateValues(existingAction, obj);
                return await this.Put(existingAction.Id, existingAction);
            }
            // else create new
            return await base.Post(obj);
        }
        [Route("disagree")]
        [HttpPost]
        public async Task<IActionResult> Disagree([FromBody] ContentPostActionsEntity obj)
        {
            var existingAction = _service.Find(x => x.Id == obj.Id).FirstOrDefault();
            obj.Agree = false;
            obj.Disagree = true;
            if (existingAction != null)
            {
                this.UpdateValues(existingAction, obj);
                return await this.Put(existingAction.Id, existingAction);
            }
            // else create new
            return await base.Post(obj);
        }
        [Route("like")]
        [HttpPost]
        public async Task<IActionResult> Like([FromBody] ContentPostActionsEntity obj)
        {
            var existingAction = _service.Find(x => x.Id == obj.Id).FirstOrDefault();

            //obj.Like = true;
            if (existingAction != null)
            {
                this.UpdateValues(existingAction, obj);
                return await this.Put(existingAction.Id, existingAction);
            }
            // else create new
            return await base.Post(obj);
        }

        [Route("unlike")]
        [HttpPost]
        public async Task<IActionResult> Unlike([FromBody] ContentPostActionsEntity obj)
        {
            var existingAction = _service.Find(x => x.Id == obj.Id).FirstOrDefault();

            //obj.Like = false;
            if (existingAction != null)
            {
                this.UpdateValues(existingAction, obj);
                return await this.Put(existingAction.Id, existingAction);
            }
            // else create new
            return await base.Post(obj);
        }

        [HttpPost]
        public override async Task<IActionResult> Post([FromBody] ContentPostActionsEntity obj)
        {
            try
            {
                var id = await _postService.CreateOrUpdatePostAction(obj);

                return Ok(new
                {
                    message = "Created Successfully",
                    id = id
                });
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }

        }
        protected override async Task PostCreateSteps(ContentPostActionsEntity obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<ContentPostActionsEntity>() { Data = obj });
        }
        protected override void UpdateValues(ContentPostActionsEntity from, dynamic to)
        {
            from.UpdatedOn = DateTime.Now;
            if(to.Like) from.Like = to.Like;
            if (to.Agree) from.Agree = to.Agree;
            if (to.Disagree) from.Disagree =  to.Disagree;
            if (to.Reaction) from.Reaction = to.Reaction;
        }
        protected override async Task PostUpdateSteps(ContentPostActionsEntity obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<ContentPostActionsEntity>() { Data = obj });
        }
        protected override async Task PostDeleteSteps(ContentPostActionsEntity obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<ContentPostActionsEntity>() { Data = obj });
        }
    }
}