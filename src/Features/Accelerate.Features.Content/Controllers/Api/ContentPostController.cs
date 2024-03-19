using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Contracts;
using Accelerate.Foundations.Integrations.Elastic.Services;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentPostController : BaseApiController<ContentPostEntity>
    {
        IPublishEndpoint _publishEndpoint;
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        //IElasticService<ContentPostEntity> _searchService;
        IContentPostElasticService _contentElasticService;
        public ContentPostController(
            IMetaContentService contentService,
            IEntityService<ContentPostEntity> service,
            IPublishEndpoint publishEndpoint,
            //IElasticService<ContentPostEntity> searchService,
            IContentPostElasticService contentElasticService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
         //   _searchService = searchService;
            _contentElasticService = contentElasticService;
        }

        [HttpPost]
        public override async Task<IActionResult> Post(ContentPostEntity obj)
        {
            var entity = await base.Post(obj);
            var userId = obj.UserId.GetValueOrDefault().ToString();
            var user = await _userManager.FindByIdAsync(userId);
            var indexModel = new ContentPost()
            {
                Content = obj.Content,
                User = user?.UserName ?? "Deleted"
            };
            var indexResponse = await _contentElasticService.Index(indexModel);
            // Emit event
            await _publishEndpoint.Publish(new GettingStarted()
            {
                Value = "Created from contentpost controller"
            });
            return entity;
        }
    }
}