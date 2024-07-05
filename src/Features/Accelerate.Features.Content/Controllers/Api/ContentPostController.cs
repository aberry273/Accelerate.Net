using Accelerate.Foundations.Content.EventBus;
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
using static Accelerate.Foundations.Database.Constants.Exceptions;
using MassTransit.Initializers;
using Accelerate.Foundations.Integrations.AzureStorage.Models;
using Accelerate.Foundations.Media.Models.Data;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Accelerate.Foundations.Content.Services;
using static Elastic.Clients.Elasticsearch.JoinField;
using System.ComponentModel;
using Accelerate.Foundations.Account.Services;
using System.Security.Policy;
using System; 

namespace Accelerate.Features.Content.Controllers.Api
{
    public class MetadataRequest
    {
        public string Url { get; set; }
    }
    public class MetadataResponse
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ContentPostController : BaseApiController<ContentPostEntity>
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IAccountUserSearchService _userSearchService;
        IElasticService<ContentPostDocument> _searchService;
        IEntityService<ContentPostQuoteEntity> _quoteService;
        IEntityService<ContentPostMediaEntity> _postMediaService;
        IContentPostService _postService;
        IMediaService _mediaService;
        public ContentPostController(
            IMetaContentService contentService,
            IContentPostService postService,
            IEntityService<ContentPostEntity> service,
            IEntityService<ContentPostQuoteEntity> quoteService,
            IEntityService<ContentPostMediaEntity> postMediaService,
            IMediaService mediaService,
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostDocument> searchService,
            IAccountUserSearchService userSearchService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _postService = postService;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
            _quoteService = quoteService;
            _postMediaService = postMediaService;
            _mediaService = mediaService;
            _mediaService = mediaService;
            _userSearchService = userSearchService;
        }
      
        [Route("metadata")]
        [HttpPost]
        public async Task<IActionResult> GetUrlContents([FromBody] MetadataRequest data)
        {
            try
            {
                bool isUri = Uri.IsWellFormedUriString(data.Url, UriKind.RelativeOrAbsolute);
                if (!isUri) return Ok(null);
                var metadata = Foundations.Common.Helpers.HtmlHelper.GetMetaDataFromUrl(data.Url);
                return Ok(metadata); 
            }
            catch(Exception e)
            {
                return Problem();
            }
        }

        #region MixedPostCreate methods
        private ContentPostSettingsEntity CreatePostSettingsFromRequest(ContentPostMixedRequest obj)
        {
            var hasSettings =
                    obj.CharLimit != null
                    || obj.WordLimit != null
                    || obj.ImageLimit != null
                    || obj.VideoLimit != null
                    || obj.QuoteLimit != null
                    || obj.Access != null;
            
            if (!hasSettings) return null;

            return new ContentPostSettingsEntity()
            {
                Access = obj.Access ?? "ALL",
                UserId = obj.UserId,
                CharLimit = obj.CharLimit,
                ImageLimit = obj.ImageLimit,
                VideoLimit = obj.VideoLimit,
                QuoteLimit = obj.QuoteLimit,
            };
        }

        private ContentPostLinkEntity CreateLinkCardFromRequest(ContentPostMixedRequest obj)
        {
            if (string.IsNullOrEmpty(obj.LinkValue)) return null;
            var linkObj = Foundations.Common.Helpers.JsonSerializerHelper.SafelyDeserializeObject<MetadataResponse>(obj?.LinkValue);
            if (linkObj == null) return null; 
            return new ContentPostLinkEntity()
            {
                Title = linkObj.Title,
                Description = linkObj.Description,
                Image = linkObj.Image,
                Url = linkObj.Url,
            };
        }

        private ContentPostTaxonomyEntity CreateTaxonomyFromRequest(ContentPostMixedRequest obj)
        {
            var tags = (obj.Tags != null) ? obj.Tags.Where(x => x != null).ToList() : new List<string>();
            return new ContentPostTaxonomyEntity()
            {
                TagItems = tags.ToList(),
                Category = obj.Category,
            };
        }

        private async Task<List<Guid>> CreateMentionsFromRequest(ContentPostMixedRequest obj)
        {
            var mentions = GetUsernames(obj.Content);

            if (!mentions.Any()) return null;
            
            var query = new RequestQuery()
            {
                Filters = new List<QueryFilter>()
                {
                    new QueryFilter()
                    {
                        Name = Foundations.Account.Constants.Fields.Username,
                        Keyword = true,
                        Condition = ElasticCondition.Filter,
                        Values = mentions,
                    }
                }
            };
            var userResults = await _userSearchService.SearchUsers(query);
            if (!userResults.Users.Any()) return null;
            return userResults.Users.Select(x => x.Id).ToList(); 
        }

        private List<Guid> CreateQuotesFromRequest(ContentPostMixedRequest obj)
        {
            return (obj.QuotedIds != null && obj.QuotedIds.Any())
                ? obj.QuotedIds.Where(x => x != Guid.Empty).ToList() 
                : new List<Guid>();
        }

        private async Task<List<Guid>> CreateMediaFromRequest(ContentPostMixedRequest obj)
        {
            var images = (obj.Images != null) ? obj.Images.Where(x => x != null).ToList() : new List<IFormFile>();
            var videos = (obj.Videos != null) ? obj.Videos.Where(x => x != null).ToList() : new List<IFormFile>();
            var media = (obj.MediaIds != null) ? obj.MediaIds.Where(x => x != Guid.Empty).ToList() : new List<Guid>();

            // Media
            // Upload formfiles, create entities from formfiles, add to request
            if (images.Any())
            {
                var mediaFiles = await _mediaService.UploadImagesFromFiles(obj.UserId, obj.Images);

                media.AddRange(mediaFiles.Select(x => x.Id));
            }
            // Upload formfiles, create entities from formfiles, add to request
            if (videos.Any())
            {
                var mediaFiles = await _mediaService.UploadVideosFromFiles(obj.UserId, obj.Videos);

                media.AddRange(mediaFiles.Select(x => x.Id));
            }
            return media;
        }

        private string MentionRegex = @"@(.*?)#u";
        private List<string> GetUsernames(string content)
        {
            var matches = Regex.Matches(content, MentionRegex);
            return matches.Where(x => x.Success)
                .Select(x => x.Groups?.Values?.LastOrDefault()?.Value)
                .ToList();
        }
        #endregion

        [Route("mixed")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Mixed([FromForm] ContentPostMixedRequest obj)
        {
            try
            {
                var parentId = obj.ParentId.GetValueOrDefault();
                var parentIds = obj.ParentIdItems?.ToList() ?? new List<Guid>();
                var channelId = obj.ChannelId.GetValueOrDefault();
                var settings = CreatePostSettingsFromRequest(obj);
                var linkCard = CreateLinkCardFromRequest(obj);
                var taxonomy = CreateTaxonomyFromRequest(obj);
                var quoteIds = CreateQuotesFromRequest(obj);
                var mentions = await this.CreateMentionsFromRequest(obj);
                var mediaIds = await CreateMediaFromRequest(obj);

                var post = await _postService.CreatePost(
                    obj,
                    obj.ChannelId.GetValueOrDefault(),
                    obj.ParentId.GetValueOrDefault(),
                    parentIds,
                    mentions,
                    quoteIds,
                    mediaIds,
                    settings,
                    linkCard,
                    taxonomy
                    );

                return Ok(new
                {
                    message = "Created Successfully",
                    id = post.Id
                });
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
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

        [HttpPost]
        public override async Task<IActionResult> Post(ContentPostEntity obj)
        {
            try
            {
                var entity = await _postService.CreateWithPipeline(obj);

                if (entity == null)
                {
                    return ValidationProblem();
                } 
                return Ok(new
                {
                    message = "Created Successfully",
                    id = entity.Id
                });
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("{id}")]
        public override async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] ContentPostEntity obj)
        {
            try
            {
                var entity = await _postService.Update(id, obj);
                if (entity == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    message = "Updated Successfully",
                    id = id
                });
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public override async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var result = await _postService.Delete(id);
                if (result == -1)
                {
                    return NotFound();
                }
                if(result > 0)
                {
                    return Ok(new
                    {
                        message = "Delete Successfully",
                        id = id
                    });
                }
                return BadRequest(new
                {
                    message = "Delete unsuccessful",
                    id = id
                });
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
        }

        private QueryDescriptor<ContentPostDocument> GetPostsQuery()
        {
            var query = new QueryDescriptor<ContentPostDocument>();
            query.MatchAll();
            return query;
        } 
        protected override async Task PostCreateSteps(ContentPostEntity obj)
        {
            await _postService.RunCreatePipeline(obj);
        }
        protected override async Task PostUpdateSteps(ContentPostEntity obj)
        {
            await _postService.RunUpdatePipeline(obj);
        }
        protected override async Task PostDeleteSteps(ContentPostEntity obj)
        {
            await _postService.RunDeletePipeline(obj);
        }
        protected override void UpdateValues(ContentPostEntity from, dynamic to)
        {
            from.Content = to.Content;
        }
    }
}