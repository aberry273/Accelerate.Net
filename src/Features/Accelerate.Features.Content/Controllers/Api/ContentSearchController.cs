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
        UserManager<AccountUser> _userManager;
        IContentViewService _contentService;
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IContentPostElasticService _searchService;
        //IElasticService<ContentPostActionDocument> _searchActionService;
        IAccountUserSearchService _userSearchService;
        IElasticService<ContentChannelDocument> _searchChannelService;
        IEntityService<ContentPostPinEntity> _pinnedContentService;
        public ContentSearchController(
            IContentViewService contentService,
            IContentPostElasticService service,
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostDocument> searchPostService,
            IAccountUserSearchService userSearchService,
            //IElasticService<ContentPostActionDocument> searchActionService,
            IEntityService<ContentPostPinEntity> pinnedContentService,
            IElasticService<ContentChannelDocument> searchChannelService,
            UserManager<AccountUser> userManager)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = service;
            //_searchActionService = searchActionService;
            _userSearchService = userSearchService;
            _searchChannelService = searchChannelService;
            _pinnedContentService = pinnedContentService;
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
            var sortBy = _contentService.GetSortField(query.Filters);
            var sortOrder = _contentService.GetSortOrderField(query.Filters);
            query.Filters = _contentService.GetActualFilterKeys(query.Filters);
            var result = await _searchService.SearchPost(query, postId, GetSortField(query), sortOrder);
            return Ok(result);
        }
        [Route("Posts/{userId}")]
        [HttpPost]
        public async Task<IActionResult> SearchUserPosts([FromBody] RequestQuery query, [FromRoute] Guid userId)
        {
            query.Filters = _contentService.GetActualFilterKeys(query.Filters);
            var result = await _searchService.SearchUserPosts(userId, query.Page, query.ItemsPerPage);
            return Ok(result);
        }
        [Route("Posts/{postId}/Parents")]
        [HttpPost]
        public async Task<IActionResult> SearchPostParents([FromRoute] Guid postId, [FromBody] RequestQuery query)
        {
            query.Filters = _contentService.GetActualFilterKeys(query.Filters);
            var result = await _searchService.SearchPostParents(query, postId, query.UserId.GetValueOrDefault());
            return Ok(result);
        }


        [Route("Posts/pinned/{postId}")]
        [HttpPost]
        public async Task<IActionResult> SearchPostPinned([FromRoute] Guid postId, [FromBody] RequestQuery query)
        {
            var pinned = _pinnedContentService.Find(x => x.ContentPostId == postId);
            var pinnedIds = pinned.Select(x => x.PinnedContentPostId.ToString()).ToList();
            query.ItemsPerPage = 10;
            query.Page = 0;
            var posts = await _searchService.SearchPostByIds(query, pinnedIds);
            var userIds = pinned.Select(x => x.UserId.ToString()).ToList();
            var users = await _userSearchService.SearchUsers(query, userIds);

            var pinnedDocuments = pinned.Select(x =>
            {
                var post = posts.FirstOrDefault(y => y.Id == x.PinnedContentPostId);
                var user = users.Users.FirstOrDefault(y => y.Id == x.UserId);
                return new ContentPostPinDocument()
                {
                    ContentPost = post,
                    Date = Foundations.Common.Extensions.DateExtensions.ToDateSimple(x.CreatedOn),
                    Id = x.Id,
                    ContentPostId = x.ContentPostId,
                    Reason = x.Reason,
                    Href = post?.Href,
                    UserId = post?.UserId,
                    Username = user?.Username,
                };
            }).ToList();
            
            return Ok(pinnedDocuments);
        }
        private string GetSortField(RequestQuery query)
        {
            var sortBy = _contentService.GetFilterSortOptions().FirstOrDefault(x => x.Name == query.Sort);
            return sortBy?.Key ?? _contentService.GetFilterSortOptions().FirstOrDefault().Key;
        }
        [Route("Posts/Replies/{postId}")]
        [HttpPost]
        public async Task<IActionResult> SearchPostReplies([FromRoute] Guid postId, [FromBody] RequestQuery query)
        {
            query.Filters = _contentService.GetActualFilterKeys(query.Filters);
            SortOrder sortOrder = SortOrder.Desc;
            Enum.TryParse<SortOrder>(query.SortBy, out sortOrder);
            query.Filters = _contentService.GetActualFilterKeys(query.Filters); 
            var result = await _searchService.SearchPostReplies(postId, query, GetSortField(query), sortOrder);
            return Ok(result);
        }
        [Route("Posts")]
        [HttpPost]
        public async Task<IActionResult> SearchPosts([FromBody] RequestQuery query)
        {
            query.Filters = _contentService.GetActualFilterKeys(query.Filters);
            SortOrder sortOrder = SortOrder.Desc;
            Enum.TryParse<SortOrder>(query.SortBy, out sortOrder);
            var sortBy = _contentService.GetFilterSortOptions().FirstOrDefault(x => x.Name == query.Sort);
            var result = await _searchService.SearchPosts(query, GetSortField(query), sortOrder);
            return Ok(result);
        }
        [Route("Posts/Related/{channelId}")]
        [HttpPost]
        public async Task<IActionResult> SearchPostsRelated(Guid channelId, [FromBody] RequestQuery query)
        {
            query.Filters = _contentService.GetActualFilterKeys(query.Filters);
            var channel = await _searchChannelService.GetDocument<ContentChannelDocument>(channelId.ToString());
            var result = await _searchService.SearchRelatedPosts(channel.Source, query);
            return Ok(result);
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