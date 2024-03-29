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
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using MassTransit;
using MassTransit.DependencyInjection;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Twilio.Rest.Proxy.V1.Service.Session.Participant;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentPostController : BaseApiController<ContentPostEntity>
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        readonly Bind<IContentBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<ContentPostDocument> _searchService;
        public ContentPostController(
            IMetaContentService contentService,
            IEntityService<ContentPostEntity> service,
            Bind<IContentBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostDocument> searchService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
        }

        [HttpGet]
        public override async Task<IActionResult> Get([FromQuery] RequestQuery<ContentPostEntity> query)
        {
            var results = await _searchService.Search(GetPostsQuery(query), 0, 1000);
            //var results = await _searchService.Find(query);
            return Ok(results.Documents);
        } 
        private QueryDescriptor<ContentPostDocument> GetPostsQuery(RequestQuery<ContentPostEntity> request)
        {
            var query = new QueryDescriptor<ContentPostDocument>();
            query.MatchAll();
            if (!string.IsNullOrEmpty(request.Query?.TargetThread))
            {
                query.Term(x => x.TargetThread.Suffix("keyword"), request.Query?.TargetThread);
            }
            else
            {
                query.Bool(x => x.MustNot(y => y.Exists(z => z.Field("targetThread"))));
            }
            return query;
        }
        private string GetTarget(ContentPostEntity obj) => obj.TargetThread ?? obj.TargetChannel;
        protected override async Task PostCreateSteps(ContentPostEntity obj)
        { 
            await _publishEndpoint.Value.Publish(new CreateDataContract<ContentPostEntity>()
            {
                Data = obj,
                Target = GetTarget(obj),
                UserId = obj.UserId
            });
        }
        protected override void UpdateValues(ContentPostEntity from, dynamic to)
        {
            from.Content = to.Content;
        }
        protected override async Task PostUpdateSteps(ContentPostEntity obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<ContentPostEntity>()
            {
                Data = obj,
                Target = GetTarget(obj),
                UserId = obj.UserId
            });
        }
        protected override async Task PostDeleteSteps(ContentPostEntity obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<ContentPostEntity>()
            {
                Data = obj,
                Target = GetTarget(obj),
                UserId = obj.UserId
            });
        }
    }
}