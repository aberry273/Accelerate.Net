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
using Accelerate.Foundations.EventPipelines.Controllers;
using Microsoft.AspNetCore.SignalR;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using Accelerate.Foundations.EventPipelines.Services;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentPostLabelController : BaseApiPipelineController<ContentPostLabelEntity, IContentPostLabelBus>
    { 
        UserManager<UsersUser> _userManager;
        IMetaContentService _contentService;
        IElasticService<ContentPostDocument> _searchService;
        IEntityService<ContentPostEntity> _postService;
        public ContentPostLabelController(
            IMetaContentService contentService,
            IEntityPipelineService<ContentPostLabelEntity, IContentPostLabelBus> service,
            IEntityService<ContentPostEntity> postService,
            IElasticService<ContentPostDocument> searchService,
            UserManager<UsersUser> userManager) : base(service)
        {
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
            _postService = postService;
        }
        protected override void UpdateValues(ContentPostLabelEntity from, dynamic to)
        {
            from.Label = to.Label;
            from.Reason = to.Reason;
        }
        [Route("post/{contentPostId}")]
        [HttpPost]
        public async Task<IActionResult> Label([FromRoute] Guid contentPostId, [FromBody] ContentPostLabelEntity obj)
        {
            obj.ContentPostId = contentPostId;
            var id = await _service.CreateWithGuid(obj);

            if (id == null)
            {
                return ValidationProblem();
            }
            //To override
            var entity = _service.Get(id.GetValueOrDefault());
            return Ok(new
            {
                message = "Created Successfully",
                entity
            });
        }
    }
}