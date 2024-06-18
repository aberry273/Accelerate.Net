﻿using Accelerate.Foundations.Content.EventBus;
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
using Accelerate.Foundations.Media.Models.Data;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Accelerate.Foundations.Content.Services;
using static Elastic.Clients.Elasticsearch.JoinField;

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
        public string Description { get; set; }
        public string Image { get; set; }
    }
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
        IContentPostService _postService;
        IMediaService _mediaFileService;
        IEntityService<MediaBlobEntity> _mediaService;
        public ContentPostController(
            IMetaContentService contentService,
            IContentPostService postService,
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
            this._postService = postService;
            _userManager = userManager;
            _contentService = contentService;
            _searchService = searchService;
            _quoteService = quoteService;
            _postMediaService = postMediaService;
            _mediaFileService = mediaFileService;
            _mediaService = mediaService;
        }
        //<meta\b[^>]*\bname=["]keywords["][^>]*\bcontent=(['"]?)((?:[^,>"'],?){1,})\1[>]
        private string UrlTagRegex = @"\<meta\b[^>]*\bproperty=[""]og:url[""][^>]*\bcontent=(['""]?)((?:[^,>""'],?){1,})\1[>]";
        private string TitleTagRegex = @"\<meta\b[^>]*\bproperty=[""]og:title[""][^>]*\bcontent=(['""]?)((?:[^,>""'],?){1,})\1[>]";
        private string DescriptionTagRegex = @"\<meta\b[^>]*\bproperty=[""]og:description[""][^>]*\bcontent=(['""]?)((?:[^,>""'],?){1,})\1[>]";
        private string ImageTagRegex = @"\<meta\b[^>]*\bproperty=[""]og:image[""][^>]*\bcontent=(['""]?)((?:[^,>""'],?){1,})\1[>]";
        private string GetTagMetadata(string input, string regex)
        {
            var result = Regex.Match(input, regex, RegexOptions.IgnoreCase);
            if (result == null) return null;
            return result.Groups[2].Value;
            
        }
        [Route("metadata")]
        [HttpPost]
        public async Task<IActionResult> GetUrlContents([FromBody] MetadataRequest data)
        {
            try
            {

                const int bytesToRead = 12000;
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(data.Url))
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    var partialHtml = Encoding.UTF8.GetString(buffer);

                    var title = GetTagMetadata(partialHtml, TitleTagRegex);
                    var description = GetTagMetadata(partialHtml, DescriptionTagRegex);
                    var image = GetTagMetadata(partialHtml, ImageTagRegex);
                    var url = GetTagMetadata(partialHtml, UrlTagRegex);

                    var model = new MetadataResponse()
                    {
                        Url = url,
                        Title = title,
                        Description = description,
                        Image = image,
                    };
                    return Ok(model);
                }
            }
            catch(Exception e)
            {
                return Problem();
            }
        }

        [Route("mixed")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Mixed([FromForm] ContentPostMixedRequest obj)
        {
            try
            {
                var parentId = obj.ParentId.GetValueOrDefault();
                var link = obj.Link;
                var settingsStr = obj.Settings;
                var quotes = (obj.QuotedItems != null) ? obj.QuotedItems.Where(x => x != null).ToList() : new List<string>();
                var media = (obj.MediaIds != null) ? obj.MediaIds.Where(x => x != Guid.Empty).ToList() : new List<Guid>();
                var images = (obj.Images != null) ? obj.Images.Where(x => x != null).ToList() : new List<IFormFile>();
                var videos = (obj.Videos != null) ? obj.Videos.Where(x => x != null).ToList() : new List<IFormFile>();
                var mentions = (obj.Mentions != null) ? obj.Mentions.Where(x => x != null).ToList() : new List<string>();

                // IF only a direct content post, just run the create fuction
                if (parentId == Guid.Empty && settingsStr == null && link == null && !quotes.Any() && !media.Any() && !images.Any() && !videos.Any() && !mentions.Any())
                {
                    return await this.Post(obj);
                }

                var post = await _postService.Create(obj);
                if (post == null)
                {
                    return BadRequest("Unable to create post");
                }

                // Parents
                if(parentId != null)
                {
                    var ancestorIds = obj.ParentIdItems.Any() ? obj.ParentIdItems.ToList() : new List<Guid>();
                    await _postService.CreateParentPost(obj, parentId, ancestorIds);
                }
                // Links
                if (link != null)
                {
                    var linkObj = Foundations.Common.Helpers.JsonSerializerHelper.DeserializeObject<MetadataResponse>(link);
                    var linkEntity = new ContentPostLinkEntity()
                    {
                        ContentPostId = post.Id,
                        Title = linkObj.Title,
                        Description = linkObj.Description,
                        Image = linkObj.Image,
                        Url = linkObj.Url,
                    };
                    await _postService.CreateLink(linkEntity);
                }
                // Parents
                if (settingsStr != null)
                {
                    var settingsRequest = Foundations.Common.Helpers.JsonSerializerHelper.DeserializeObject<ContentPostSettingsRequest>(settingsStr);
                    
                    var settings = new ContentPostSettingsEntity()
                    {
                        Access = settingsRequest.Access ?? "All",
                        CharLimit = settingsRequest.CharLimit.GetValueOrDefault(),
                        ImageLimit = settingsRequest.ImageLimit.GetValueOrDefault(),
                        VideoLimit = settingsRequest.VideoLimit.GetValueOrDefault(),
                        FormatItems = settingsRequest.Formats.ToList(),
                        PostLimit = settingsRequest.PostLimit.GetValueOrDefault(),
                        UserId = post.UserId.GetValueOrDefault(),
                    };
                    await _postService.CreateSettings(post.Id, settings);
                }

                // Quotes
                if (quotes.Any())
                {
                    var quotesItems = obj
                        .QuotedItems
                        .Select(x => Foundations.Common.Helpers.JsonSerializerHelper.DeserializeObject<ContentPostQuoteRequest>(x))
                        .Select(x => CreateQuoteLink(post, x))
                        .ToList();

                    var quoteResults = _quoteService.AddRange(quotesItems);
                }
                // Upload formfiles, create entities from formfiles, add to request
                if (images.Any())
                {
                    var mediaFiles = await UploadImagesFromFiles(obj.UserId.GetValueOrDefault(), obj.Images);
                    
                    media.AddRange(mediaFiles.Select(x => x.Id));
                }
                // Upload formfiles, create entities from formfiles, add to request
                if (videos.Any())
                {
                    var mediaFiles = await UploadVideosFromFiles(obj.UserId.GetValueOrDefault(), obj.Videos);

                    media.AddRange(mediaFiles.Select(x => x.Id));
                }
                // for all guids sent through in request, create aa link
                if (media.Any())
                {
                    var mediaItems = media
                        .Select(x => _postService.CreateMediaLink(post, x))
                        .ToList();

                    var quoteResults = _postMediaService.AddRange(mediaItems);
                }

                await _postService.RunCreatePipeline(post);
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
                var entity = await _postService.Create(obj);

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

        private async Task<List<MediaBlobUploadResult>> UploadImagesFromFiles(Guid userId,  List<IFormFile> files)
        {
            if (files == null) return new List<MediaBlobUploadResult>();

            // upload file 
            var newFiles = files.Select(x => new MediaBlobUploadRequest(x)).ToList();
            // bulk upload with preset ids
            var fileResults = await _mediaFileService.UploadImages(userId, newFiles);

            // bulk create entities
            var mediaBlobEntities = newFiles.Select(x =>
            {
                return new MediaBlobEntity()
                {
                    Id = x.Id,
                    FilePath = _mediaFileService.GetFileUrl(x.Id.ToString()),
                    Name = x.File.FileName,
                    UserId = userId,
                    Type = MediaBlobFileType.Image
                };
            }).ToList();
            // TODO return IDs of all created entities rather than count
            var blobEntityGuids = await this._mediaService.AddRange(mediaBlobEntities);

            return fileResults.ToList();
        }

        private async Task<List<MediaBlobUploadResult>> UploadVideosFromFiles(Guid userId, List<IFormFile> files)
        {
            if (files == null) return new List<MediaBlobUploadResult>();

            // upload file 
            var newFiles = files.Select(x => new MediaBlobUploadRequest(x)).ToList();
            // bulk upload with preset ids
            var fileResults = await _mediaFileService.UploadVideos(userId, newFiles);

            // bulk create entities
            var mediaBlobEntities = newFiles.Select(x =>
            {
                return new MediaBlobEntity()
                {
                    Id = x.Id,
                    FilePath = _mediaFileService.GetFileUrl(x.Id.ToString()),
                    Name = x.File.FileName,
                    UserId = userId,
                    Type = MediaBlobFileType.Video
                };
            }).ToList();
            // TODO return IDs of all created entities rather than count
            var blobEntityGuids = await this._mediaService.AddRange(mediaBlobEntities);

            return fileResults.ToList();
        }
        private ContentPostQuoteEntity CreateQuoteLink(ContentPostEntity post, ContentPostQuoteRequest quote)
        {
            // append current thread to path of new quote, or set as default path 
            return new ContentPostQuoteEntity()
            {
                QuotedContentPostId = quote.QuotedContentPostId,
                ContentPostId = post.Id,
                Content = quote.Content,
                Response = quote.Response
            };
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
        protected override void UpdateValues(ContentPostEntity from, dynamic to)
        {
            from.Content = to.Content;
        }
        protected override async Task PostUpdateSteps(ContentPostEntity obj)
        {
            await _postService.RunUpdatePipeline(obj);
        }
        protected override async Task PostDeleteSteps(ContentPostEntity obj)
        {
            await _postService.RunDeletePipeline(obj);
        }
    }
}