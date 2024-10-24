using Accelerate.Foundations.Content.EventBus;
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
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
using Microsoft.Extensions.Primitives;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentSearchController : ControllerBase
    {
        IContentViewSearchService _contentViewSearchService;
        IContentPostElasticService _searchService;
        public ContentSearchController(
            IContentPostElasticService service,
            IContentViewSearchService contentViewSearchService,
            IEntityService<ContentPostPinEntity> pinnedContentService)
        {
            _searchService = service;
            _contentViewSearchService = contentViewSearchService;
        }
        [Route("Actions")]
        [HttpPost]
        public async Task<IActionResult> SearchUserActions([FromBody] RequestQuery query)
        {
            var docs = await _searchService.SearchUserActions(query);
            return Ok(docs);
        }
        [Route("Post/{postId}")]
        [HttpPost]
        public async Task<IActionResult> SearchPost([FromBody] RequestQuery query, [FromRoute] Guid postId)
        {
            return Ok(await _contentViewSearchService.SearchPosts(query, postId));
        }
        [Route("Posts/{userId}")]
        [HttpPost]
        public async Task<IActionResult> SearchUserPosts([FromBody] RequestQuery query, [FromRoute] Guid userId)
        {
            return Ok(await _contentViewSearchService.SearchUserPosts(query, userId));
        }
        [Route("Posts/{postId}/Parents")]
        [HttpPost]
        public async Task<IActionResult> SearchPostParents([FromRoute] Guid postId, [FromBody] RequestQuery query)
        {
            return Ok(await _contentViewSearchService.SearchPostParents(postId, query));
        }


        [Route("Posts/pinned/{postId}")]
        [HttpPost]
        public async Task<IActionResult> SearchPostPinned([FromRoute] Guid postId, [FromBody] RequestQuery query)
        {
            return Ok(await _contentViewSearchService.SearchPostPinned(postId, query));
        }
        [Route("Posts/Replies/{postId}")]
        [HttpPost]
        public async Task<IActionResult> SearchPostRepliesFromRoute([FromRoute] Guid postId, [FromBody] RequestQuery query)
        {
            return Ok(await _contentViewSearchService.SearchPostRepliesFromRoute(postId, query));
        }
        [Route("Posts/Replies")]
        [HttpPost]
        public async Task<IActionResult> SearchPostReplies([FromBody] RequestQuery query)
        {
            return Ok(await _contentViewSearchService.SearchPostReplies(query));
        }
        [Route("Posts")]
        [HttpPost]
        public async Task<IActionResult> SearchPosts([FromBody] RequestQuery query)
        {
            return Ok(await _contentViewSearchService.SearchPosts(query));
        }
        [Route("Posts/Feed")]
        [HttpPost]
        public async Task<IActionResult> SearchFeedPosts([FromBody] RequestQuery query)
        {
            return Ok(await _contentViewSearchService.SearchFeedPosts(query));
        }
        [Route("Posts/Related/{channelId}")]
        [HttpPost]
        public async Task<IActionResult> SearchPostsRelated(Guid channelId, [FromBody] RequestQuery query)
        {
            return Ok(await _contentViewSearchService.SearchPostsRelated(channelId, query));
        }
        [Route("Channels")]
        [HttpPost]
        public async Task<IActionResult> SearchChannels([FromBody] RequestQuery query)
        {
            return Ok(await _contentViewSearchService.SearchChannels(query));
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