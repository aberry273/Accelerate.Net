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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Twilio.Rest.Proxy.V1.Service.Session.Participant;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentSearchController : ControllerBase
    {
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        readonly Bind<IContentBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<ContentPostDocument> _searchService;
        public ContentSearchController(
            IMetaContentService contentService,
            IEntityService<ContentPostEntity> service,
            Bind<IContentBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostDocument> searchService,
            UserManager<AccountUser> userManager)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestQuery Query)
        {
            var elasticQuery = GetPostsQuery(Query);
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : Foundations.Content.Constants.Search.DefaultPerPage;
            if (take > Foundations.Content.Constants.Search.MaxQueryable) take = Foundations.Content.Constants.Search.MaxQueryable;
            int skip = take * Query.Page;
            var results = await _searchService.Search(elasticQuery, skip, take);
            return Ok(results.Documents);
        }
        private QueryDescriptor<ContentPostDocument> GetPostsQuery(RequestQuery request)
        {
            var query = new QueryDescriptor<ContentPostDocument>();
            query.MatchAll();
            if (request.Filters.ContainsKey(Foundations.Content.Constants.Fields.TargetThread))
            {
                query.Term(x => 
                    x.TargetThread.Suffix("keyword"), 
                    request.Filters[Foundations.Content.Constants.Fields.TargetThread]?.FirstOrDefault()
                );
            }
            else
            {
                query.Bool(x => x.MustNot(y => y.Exists(z => z.Field(Foundations.Content.Constants.Fields.TargetThread))));
            }
            return query;
        }
    }
}