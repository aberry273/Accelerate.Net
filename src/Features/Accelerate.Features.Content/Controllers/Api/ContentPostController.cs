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
using Accelerate.Foundations.Media.Models.Entities;
using Accelerate.Foundations.Media.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using MassTransit;
using MassTransit.DependencyInjection;
using MassTransit.Transports;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Accelerate.Foundations.Common.Extensions;
using Microsoft.AspNetCore.SignalR;
using MimeKit.Cryptography;
using System.Threading;
using Twilio.Rest.Proxy.V1.Service.Session.Participant;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using MassTransit.Initializers;
using Accelerate.Foundations.Integrations.AzureStorage.Models;

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
        IEntityService<ContentPostMediaEntity> _postMediaService;
        IMediaService _mediaFileService;
        IEntityService<MediaBlobEntity> _mediaService;
        public ContentPostController(
            IMetaContentService contentService,
            IEntityService<ContentPostEntity> service,
            IEntityService<ContentPostQuoteEntity> quoteService,
            IEntityService<ContentPostMediaEntity> postMediaService,
            IMediaService mediaFileService,
            IEntityService<MediaBlobEntity> mediaService,
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostDocument> searchService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
            _quoteService = quoteService;
            _postMediaService = postMediaService;
            _mediaFileService = mediaFileService;
            _mediaService = mediaService;
        }

        [Route("mixed")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Mixed([FromForm] ContentPostMixedRequest obj)
        {
            try
            {
                var hasQuotes = (obj.QuoteIds != null && obj.QuoteIds.Any());
                var hasMedia = (obj.MediaIds != null && obj.MediaIds.Any());
                var hasImages = (obj.Images != null && obj.Images.Any());
                if (!hasQuotes && !hasMedia && !hasImages)
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

                if (hasQuotes)
                {
                    var quotes = obj
                        .QuoteIds
                        .Select(x => CreateQuoteLink(post, x))
                        .ToList();

                    var quoteResults = _quoteService.AddRange(quotes);
                }
                // Upload formfiles, create entities from formfiles, add to request
                if(obj.Images != null)
                {
                    var user = await _userManager.FindByIdAsync(obj.UserId.ToString());
                    var mediaFileIds = await UploadImagesFromFiles(user, obj.Images);
                    
                    if(obj.MediaIds == null)
                    {
                        obj.MediaIds = new List<Guid>();
                    }
                    obj.MediaIds.AddRange(mediaFileIds.Select(x => x));

                    hasMedia = true;
                }
                // for all guids sent through in request, create aa link
                if (hasMedia)
                {
                    var media = obj
                        .MediaIds
                        .Select(x => CreateMediaLink(post, x))
                        .ToList();

                    var quoteResults = _postMediaService.AddRange(media);
                }

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

        private async Task<List<Guid>> UploadImagesFromFiles(AccountUser user, List<IFormFile> files)
        {
            if (files == null) return new List<Guid>();
            
            // upload file 
            var newFiles = files.ToDictionary(x => Guid.NewGuid(), x => x);
            // bulk upload with preset ids
            var fileIds = await _mediaFileService.UploadImages(user.Id, newFiles);

            // bulk create entities
            var mediaBlobEntities = newFiles.Select(x =>
            {
                return new MediaBlobEntity()
                {
                    Id = x.Key,
                    FilePath = _mediaFileService.GetFileUrl(x.Key.ToString()),
                    Name = x.Value.FileName,
                    UserId = user.Id
                };
            });
            // TODO return IDs of all created entities rather than count
            var blobEntityGuids = await this._mediaService.AddRange(mediaBlobEntities);

            return newFiles.Keys.ToList();
        }

        private ContentPostQuoteEntity CreateQuoteLink(ContentPostEntity post, string threadId)
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

        private ContentPostMediaEntity CreateMediaLink(ContentPostEntity post, Guid mediaId)
        {
            return new ContentPostMediaEntity()
            {
                MediaBlobId = mediaId,
                ContentPostId = post.Id,
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