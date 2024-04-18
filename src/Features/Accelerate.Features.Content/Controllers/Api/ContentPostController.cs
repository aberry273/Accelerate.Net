using Accelerate.Features.Content.EventBus;
using Accelerate.Features.Content.Models.Data;
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
using MimeKit.Cryptography;
using System.Threading;
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
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<ContentPostDocument> _searchService;
        IEntityService<ContentPostQuoteEntity> _quoteService;
        public ContentPostController(
            IMetaContentService contentService,
            IEntityService<ContentPostEntity> service,
            IEntityService<ContentPostQuoteEntity> quoteService,
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostDocument> searchService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
            _quoteService = quoteService;
        }

        [Route("quote")]
        [HttpPost]
        public async Task<IActionResult> Quote([FromBody] ContentPostQuoteRequest obj)
        {
            try
            {
                obj.Type = ContentPostType.Reply;
                if (obj.QuoteIds == null || !obj.QuoteIds.Any())
                {
                    return await base.Post(obj);
                }

                //Create post
                var postId = await _service.CreateWithGuid(obj);
                if (postId == null)
                {
                    return BadRequest("Unable to create post");
                }

                var post = _service.Get(postId.GetValueOrDefault());

                var quotes = obj
                    .QuoteIds
                    .Select(x => CreateQuote(post, x))
                    .ToList();
               
                var quoteResults = _quoteService.AddRange(quotes);
                
                await PostCreateSteps(post);

                return Ok(new
                {
                    message = "Created Successfully",
                    id = postId
                });
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
        }

        private ContentPostQuoteEntity CreateQuote(ContentPostEntity post, string threadId)
        {
            var quotedId = Foundations.Common.Extensions.GuidExtensions.FromBase64(threadId);
            // append current thread to path of new quote, or set as default path
            var value = Foundations.Common.Extensions.GuidExtensions.ShortenBase64(threadId);
            var path = $"{Foundations.Common.Extensions.GuidExtensions.ShortenBase64(post.ThreadId)}, {Foundations.Common.Extensions.GuidExtensions.ShortenBase64(threadId)}";
            return new ContentPostQuoteEntity()
            {
                QuotedContentPostId = quotedId,
                QuoterContentPostId = post.Id,
                Path = path,
                Value = value,
            };
        }

        //Override with elastic search instead of db query
        [HttpGet]
        public override async Task<IActionResult> Get([FromQuery] int Page = 0, [FromQuery] int ItemsPerPage = 10, [FromQuery] string? Text = null)
        {
            int take = ItemsPerPage > 0 ? ItemsPerPage : 10;
            if (take > 100) take = 100;
            int skip = take * Page;
            var results = await _searchService.Search(GetPostsQuery(), skip, take);
            return Ok(results.Documents);
        }
        private QueryDescriptor<ContentPostDocument> GetPostsQuery()
        {
            var query = new QueryDescriptor<ContentPostDocument>();
            query.MatchAll();
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