using Accelerate.Features.Content.EventBus;
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
    public class ContentPostReviewController : BaseApiController<ContentPostReviewEntity>
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        readonly Bind<IContentReviewBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<ContentPostDocument> _searchService;
        IEntityService<ContentPostEntity> _postService;
        public ContentPostReviewController(
            IMetaContentService contentService,
            IEntityService<ContentPostReviewEntity> service,
            IEntityService<ContentPostEntity> postService,
            Bind<IContentReviewBus, IPublishEndpoint> publishEndpoint,
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
        public async Task<IActionResult> Agree([FromBody] ContentPostReviewEntity obj)
        {
            var existingReview = _service.Find(x => x.Id == obj.Id).FirstOrDefault();
            obj.Agree = true;
            obj.Disagree = false;
            if (existingReview != null)
            {
                this.UpdateValues(existingReview, obj);
                return await this.Put(existingReview.Id, existingReview);
            }
            // else create new
            return await base.Post(obj);
        }
        [Route("disagree")]
        [HttpPost]
        public async Task<IActionResult> Disagree([FromBody] ContentPostReviewEntity obj)
        {
            var existingReview = _service.Find(x => x.Id == obj.Id).FirstOrDefault();
            obj.Agree = false;
            obj.Disagree = true;
            if (existingReview != null)
            {
                this.UpdateValues(existingReview, obj);
                return await this.Put(existingReview.Id, existingReview);
            }
            // else create new
            return await base.Post(obj);
        }
        [Route("like")]
        [HttpPost]
        public async Task<IActionResult> Like([FromBody] ContentPostReviewEntity obj)
        {
            var existingReview = _service.Find(x => x.Id == obj.Id).FirstOrDefault();

            obj.Like = true;
            if (existingReview != null)
            {
                this.UpdateValues(existingReview, obj);
                return await this.Put(existingReview.Id, existingReview);
            }
            // else create new
            return await base.Post(obj);
        }

        [Route("unlike")]
        [HttpPost]
        public async Task<IActionResult> Unlike([FromBody] ContentPostReviewEntity obj)
        {
            var existingReview = _service.Find(x => x.Id == obj.Id).FirstOrDefault();

            obj.Like = false;
            if (existingReview != null)
            {
                this.UpdateValues(existingReview, obj);
                return await this.Put(existingReview.Id, existingReview);
            }
            // else create new
            return await base.Post(obj);
        }

        [HttpPost]
        public override async Task<IActionResult> Post([FromBody] ContentPostReviewEntity obj)
        {
            var existingReview = _service.Find(x => x.UserId == obj.UserId).FirstOrDefault();
            if (existingReview != null)
            {
                //If exists, edit existing
                //If neither set on request, use existing (user only likes)
                if(obj.Agree == null && obj.Disagree == null)
                {
                    obj.Agree = existingReview.Agree;
                    obj.Disagree = existingReview.Disagree;
                }
                //if both are set to true, set disagree to false and agree to true;
                if (obj.Agree.GetValueOrDefault() && obj.Disagree.GetValueOrDefault())
                {
                    obj.Disagree = null;
                }
                this.UpdateValues(existingReview, obj);
                return await this.Put(existingReview.Id, existingReview);
            }
            // else create new
            return await base.Post(obj);
        }

        protected override async Task PostCreateSteps(ContentPostReviewEntity obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<ContentPostReviewEntity>() { Data = obj });
        }
        protected override void UpdateValues(ContentPostReviewEntity from, dynamic to)
        {
            from.UpdatedOn = DateTime.Now;
            from.Like = to.Like != null ? to.Like : null;
            from.Agree = to.Disagree != null ? !to.Disagree : to.Agree;
            from.Disagree = to.Agree != null ? !to.Agree : to.Disagree;
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