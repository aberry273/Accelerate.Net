using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using MassTransit.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Accelerate.Foundations.Database.Services;
using Accelerate.Features.Content.Pipelines.Actions;
using Accelerate.Foundations.Content.Hydrators;
using Accelerate.Foundations.Media.Models.Entities;
using Accelerate.Foundations.Content.Services;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostCreatedPipeline : DataCreateEventPipeline<ContentPostEntity>
    {
        IContentPostService _contentPostService;
        IElasticService<AccountUserDocument> _accountElasticService;
        IElasticService<ContentPostDocument> _elasticService;
        IEntityService<ContentPostQuoteEntity> _quoteService;
        IEntityService<ContentChannelEntity> _channelService;
        IEntityService<MediaBlobEntity> _mediaService;
        IEntityService<ContentPostMediaEntity> _mediaPostService;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHub;
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IEntityService<ContentPostEntity> _entityService;
        public ContentPostCreatedPipeline(
            IContentPostService contentPostService,
            IElasticService<ContentPostDocument> elasticService,
            IEntityService<ContentChannelEntity> channelService,
            IEntityService<ContentPostEntity> entityService,
            IEntityService<ContentPostQuoteEntity> quoteService,
            IEntityService<MediaBlobEntity> mediaService,
            IEntityService<ContentPostMediaEntity> mediaPostService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHub,
            IElasticService<AccountUserDocument> accountElasticService)
        {
            _contentPostService = contentPostService;
            _entityService = entityService;
            _elasticService = elasticService;
            _messageHub = messageHub;
            _quoteService = quoteService;
            _mediaService = mediaService;
            _mediaPostService = mediaPostService;
            _channelService = channelService;
            _accountElasticService = accountElasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                IndexDocument,
            };
            _processors = new List<PipelineProcessor<ContentPostEntity>>()
            {
            };
        }
        private List<ContentPostMediaSubdocument> GetMedia(IPipelineArgs<ContentPostEntity> args)
        {
            var mediaLinks = _mediaPostService
                .Find(x => x.ContentPostId == args.Value.Id)
                .Select(x => x.MediaBlobId)
                .ToList();
            var media = _mediaService.Find(x => mediaLinks.Contains(x.Id));
            var mediaItems = media.Select(x => new ContentPostMediaSubdocument(){
                FilePath = x.FilePath,
                Type = Enum.GetName(x.Type),
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();
            return mediaItems;
        }
        private ContentPostSettingsSubdocument GetSettings(IPipelineArgs<ContentPostEntity> args)
        {
            var settings = _contentPostService.GetSettings(args.Value.Id);
            if (settings == null) return null;
         
            return new ContentPostSettingsSubdocument()
            {
                Access = settings.Access,
                CharLimit = settings.CharLimit,
                PostLimit = settings.PostLimit,
                Formats = settings.FormatItems,
                ImageLimit = settings.ImageLimit,
                //QuoteLimit = settings.QuoteLimit,
                VideoLimit = settings.VideoLimit,
            };
        }
        private ContentPostLinkSubdocument GetLink(IPipelineArgs<ContentPostEntity> args)
        {
            var link = _contentPostService.GetLink(args.Value.Id);
            if (link == null) return null;

            return new ContentPostLinkSubdocument()
            {
                Title = link.Title,
                Description = link.Description,
                Image = link.Image,
                ShortUrl = link.ShortUrl,
                Url = link.Url
            };
        }
        private List<ContentPostQuoteSubdocument> GetQuotes(IPipelineArgs<ContentPostEntity> args)
        {
            var quotes = _quoteService.Find(x => x.ContentPostId == args.Value.Id);
            
            return quotes.Select(x => new ContentPostQuoteSubdocument()
            {
                ContentPostQuoteThreadId = GuidExtensions.ShortenBase64(x.QuotedContentPostId.ToBase64()),
                ContentPostQuoteId = x.QuotedContentPostId.ToString(),
                Content = x.Content,
                Response = x.Response
            }).ToList();
        }
        private ContentPostParentEntity? GetParent(IPipelineArgs<ContentPostEntity> args)
        {
            return _contentPostService.GetPostParent(args.Value);
        }
        private string? GetChannelName(IPipelineArgs<ContentPostEntity> args)
        {
            var channel = _contentPostService.GetPostChannel(args.Value);
            return channel?.Name;
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostEntity> args)
        {
            try
            {
                var user = await _accountElasticService.GetDocument<AccountUserDocument>(args.Value.UserId.GetValueOrDefault().ToString());
                var indexModel = new ContentPostDocument();
                var profile = user.Source != null
                    ? new ContentPostUserSubdocument()
                    {
                        Username = user?.Source?.Username,
                        Image = user?.Source?.Image
                    } : null;

                args.Value.Hydrate(indexModel, profile);

                indexModel.ChannelName = GetChannelName(args);
                indexModel.QuotedPosts = GetQuotes(args);
                indexModel.Media = GetMedia(args);
                indexModel.Link = GetLink(args);
                indexModel.Settings = GetSettings(args);
                // If a reply
                var postParent = GetParent(args);
                if (postParent != null && postParent.ParentId != null)
                {
                    indexModel.ParentId = postParent.ParentId;
                    indexModel.ParentIds = postParent.ParentIdItems.ToList();

                    await UpdateParentDocument(postParent, indexModel, args);
                }
                await _elasticService.Index(indexModel);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task UpdateParentDocument(ContentPostParentEntity parentEntity, ContentPostDocument childDoc, IPipelineArgs<ContentPostEntity> args)
        {
            var parentResponse = await _elasticService.GetDocument<ContentPostDocument>(parentEntity.ParentId?.ToString());
            var parentDoc = parentResponse.Source;
            if (parentDoc == null) return;

            // Update reply count
            parentDoc.Replies = _contentPostService.GetReplyCount(parentEntity.ParentId.GetValueOrDefault());

            // Update threads
            if (args.Value.Type == ContentPostType.Page)
            {
                if (parentDoc.Pages == null) parentDoc.Pages = new List<ContentPostDocument>();
                parentDoc.Pages.Add(childDoc);
            }
            await _elasticService.UpdateDocument(parentDoc, parentDoc.Id.ToString());
        }
    }
}
