using Accelerate.Features.Content.EventBus;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Integrations.Contracts;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.TermVectors;
using Elastic.Clients.Elasticsearch.QueryDsl;
using MassTransit;
using MassTransit.DependencyInjection;
using MassTransit.Transports;
using MessagePack;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Twilio.Rest.Proxy.V1.Service.Session.Participant;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentSearchController : ControllerBase
    {
        UserManager<AccountUser> _userManager;
        IContentViewService _contentService;
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IContentPostElasticService _searchService;
        IElasticService<ContentPostReviewDocument> _searchReviewService;
        public ContentSearchController(
            IContentViewService contentService,
            IContentPostElasticService service,
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostDocument> searchPostService,
            IElasticService<ContentPostReviewDocument> searchReviewService,
            UserManager<AccountUser> userManager)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = service;
            _searchReviewService = searchReviewService;
        }
        [Route("Reviews")]
        [HttpPost]
        public async Task<IActionResult> SearchUserReviews([FromBody] RequestQuery query)
        {
            var docs = await _searchService.SearchUserReviews(query);
            return Ok(docs);
        }
        [Route("Posts")]
        [HttpPost]
        public async Task<IActionResult> SearchPosts([FromBody] RequestQuery query)
        {
            query.Filters = _contentService.GetActualFilterKeys(query.Filters);
            var docs = await _searchService.SearchPosts(query);
            return Ok(docs);
        }
        [Route("Channels")]
        [HttpPost]
        public async Task<IActionResult> SearchChannels([FromBody] RequestQuery query)
        {
            query.Filters = _contentService.GetActualFilterKeys(query.Filters);
            var docs = await _searchService.SearchPosts(query);
            return Ok(docs);
        }
        [Route("Index")]
        [HttpDelete]
        public async Task<IActionResult> DeleteIndex()
        {
            var docs = await _searchService.DeleteIndex();
            return Ok(docs);
        }
    }
}