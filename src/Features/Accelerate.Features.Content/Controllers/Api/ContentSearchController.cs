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
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<ContentPostDocument> _searchPostService;
        IElasticService<ContentPostReviewDocument> _searchReviewService;
        public ContentSearchController(
            IMetaContentService contentService,
            IEntityService<ContentPostEntity> service,
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostDocument> searchPostService,
            IElasticService<ContentPostReviewDocument> searchReviewService,
            UserManager<AccountUser> userManager)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchPostService = searchPostService;
            _searchReviewService = searchReviewService;
        }
        [Route("Reviews")]
        [HttpPost]
        public async Task<IActionResult> SearchUserReviews([FromBody] RequestQuery Query)
        {
            var elasticQuery = GetUserReviewsQuery(Query);
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : Foundations.Content.Constants.Search.DefaultPerPage;
            if (take > Foundations.Content.Constants.Search.MaxQueryable) take = Foundations.Content.Constants.Search.MaxQueryable;
            int skip = take * Query.Page;
            var results = await _searchReviewService.Search(elasticQuery, skip, take);
            if (!results.IsValidResponse || !results.IsSuccess())
            {
                return Ok(new List<ContentPostReviewDocument>());
            }
            return Ok(results.Documents);
        }
        private QueryDescriptor<ContentPostReviewDocument> GetUserReviewsQuery(RequestQuery request)
        {
            var query = new QueryDescriptor<ContentPostReviewDocument>();
            if (request.Filters != null && request.Filters.Any())
            {
                query.MatchAll();
                query.Term(x =>
                    x.UserId.Suffix("keyword"),
                    request.Filters[Foundations.Content.Constants.Fields.UserId]?.FirstOrDefault()
                );
            }
            return query;
        }
        [Route("Posts")]
        [HttpPost]
        public async Task<IActionResult> SearchPosts([FromBody] RequestQuery Query)
        {
            var elasticQuery = GetPostsQuery(Query);
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : Foundations.Content.Constants.Search.DefaultPerPage;
            if (take > Foundations.Content.Constants.Search.MaxQueryable) take = Foundations.Content.Constants.Search.MaxQueryable;
            int skip = take * Query.Page;
            var results = await _searchPostService.Search(elasticQuery, skip, take);
            if (!results.IsValidResponse && !results.IsSuccess())
            {
                return Ok(new List<ContentPostDocument>());
            }
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