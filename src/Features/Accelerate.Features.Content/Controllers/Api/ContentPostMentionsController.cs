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
using static Elastic.Clients.Elasticsearch.JoinField;
using Accelerate.Foundations.Content.Services;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentPostMentionsController : BaseApiServiceController<ContentPostMentionEntity>
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        readonly Bind<IContentActionsBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<ContentPostDocument> _searchService;
        IContentPostService _postService;
        public ContentPostMentionsController(
            IMetaContentService contentService,
            IEntityService<ContentPostMentionEntity> service,
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
         

        [HttpPost]
        public override async Task<IActionResult> Post([FromBody] ContentPostMentionEntity obj)
        {
            try
            {
                var id = await _postService.CreateMentions(obj.ContentPostId, new List<Guid>() { obj.UserId });

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
        protected override async Task PostCreateSteps(ContentPostMentionEntity obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<ContentPostMentionEntity>() { Data = obj });
        }
        protected override void UpdateValues(ContentPostMentionEntity from, dynamic to)
        {
            from.UpdatedOn = DateTime.Now; 
        }
        protected override async Task PostUpdateSteps(ContentPostMentionEntity obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<ContentPostMentionEntity>() { Data = obj });
        }
        protected override async Task PostDeleteSteps(ContentPostMentionEntity obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<ContentPostMentionEntity>() { Data = obj });
        }
    }
}