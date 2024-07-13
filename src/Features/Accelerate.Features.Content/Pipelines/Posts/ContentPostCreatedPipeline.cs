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
using Accelerate.Foundations.EventPipelines.Services;
using Accelerate.Foundations.Account.Models.Entities;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostCreatedPipeline : DataCreateEventPipeline<ContentPostEntity>
    {
        private UserManager<AccountUser> _userManager;
        IContentPostService _contentPostService;
        IElasticService<AccountUserDocument> _accountElasticService;
        IElasticService<ContentPostDocument> _elasticService;
        IElasticService<ContentPostActionsDocument> _elasticPostActionsService;
        IElasticService<ContentPostActionsSummaryDocument> _elasticPostActionsSummaryService;
        IEntityService<ContentPostQuoteEntity> _quoteService;
        IEntityService<ContentChannelEntity> _channelService;
        IEntityService<MediaBlobEntity> _mediaService;
        IEntityService<ContentPostMediaEntity> _mediaPostService;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHub;
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IEntityService<ContentPostEntity> _entityService;
        IEntityService<ContentPostActionsSummaryEntity> _entityActionsSummaryService;
        IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> _pipelineActivityService;
        public ContentPostCreatedPipeline(
            IContentPostService contentPostService,
            UserManager<AccountUser> userManager,
            IElasticService<ContentPostDocument> elasticService,
            IElasticService<ContentPostActionsDocument> elasticPostActionsService,
            IElasticService<ContentPostActionsSummaryDocument> elasticPostActionsSummaryService,
            IEntityService<ContentChannelEntity> channelService,
            IEntityService<ContentPostEntity> entityService,
            IEntityService<ContentPostQuoteEntity> quoteService,
            IEntityService<ContentPostActionsSummaryEntity> entityActionsSummaryService,
            IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> pipelineActivityService,
            IEntityService<MediaBlobEntity> mediaService,
            IEntityService<ContentPostMediaEntity> mediaPostService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHub,
            IElasticService<AccountUserDocument> accountElasticService)
        {
            _contentPostService = contentPostService;
            _elasticPostActionsService = elasticPostActionsService;
            _entityActionsSummaryService = entityActionsSummaryService;
            _pipelineActivityService = pipelineActivityService;
            _elasticPostActionsSummaryService = elasticPostActionsSummaryService;
            _entityService = entityService;
            _elasticService = elasticService;
            _messageHub = messageHub;
            _quoteService = quoteService;
            _mediaService = mediaService;
            _mediaPostService = mediaPostService;
            _userManager = userManager;
            _channelService = channelService;
            _accountElasticService = accountElasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                IndexDocument,
                //CreatePostActionSummary,
                CreatePostActivity
            };
            _processors = new List<PipelineProcessor<ContentPostEntity>>()
            {
            };
        }

        private async Task CreatePostActivity(IPipelineArgs<ContentPostEntity> args)
        {
            var entity = new ContentPostActivityEntity()
            {
                Type = ContentPostActivityTypes.Created,
                UserId = args.Value.UserId,
                Message = "Post created",
            };
            await _pipelineActivityService.Create(entity);
        }

        private async Task CreatePostActionSummary(IPipelineArgs<ContentPostEntity> args)
        {
            var entity = new ContentPostActionsSummaryEntity()
            {
                ContentPostId = args.Value.Id,
                Agrees = 0,
                Disagrees = 0,
                Replies = 0,
                Quotes = 0,
            };
            var id = await _entityActionsSummaryService.CreateWithGuid(entity);
            if (id == null) return;
            entity.Id = id.GetValueOrDefault();

            var doc = new ContentPostActionsSummaryDocument()
            {
                ContentPostId = args.Value.Id,
            };
            var response = await _elasticPostActionsSummaryService.Index(doc);
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
        private ContentPostTaxonomySubdocument GetTaxonomy(IPipelineArgs<ContentPostEntity> args)
        {
            var taxonomy = _contentPostService.GetTaxonomy(args.Value.Id);
            if (taxonomy == null) return null;

            return new ContentPostTaxonomySubdocument()
            {
                Category = taxonomy.Category,
                Tags = taxonomy.TagItems?.ToList()
            };
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
                ImageLimit = settings.ImageLimit,
                QuoteLimit = settings.QuoteLimit,
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
            }).ToList();
        }
        private ContentPostParentEntity? GetParent(IPipelineArgs<ContentPostEntity> args)
        {
            return _contentPostService.GetPostParent(args.Value);
        }
        private ContentChannelEntity? GetChannelName(IPipelineArgs<ContentPostEntity> args)
        {
            return _contentPostService.GetPostChannel(args.Value);
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostEntity> args)
        {
            try
            {
                var user = await _accountElasticService.GetDocument<AccountUserDocument>(args.Value.UserId.ToString());
                var indexModel = new ContentPostDocument();
                var profile = user.Source != null
                    ? new ContentPostUserSubdocument()
                    {
                        Username = user?.Source?.Username,
                        Image = user?.Source?.Image
                    } : null;

                args.Value.Hydrate(indexModel, profile);
                var channel = GetChannelName(args);
                indexModel.ChannelName = channel?.Name;
                indexModel.ChannelId = channel?.Id;
                indexModel.QuotedPosts = GetQuotes(args);
                indexModel.Media = GetMedia(args);
                indexModel.Link = GetLink(args);
                indexModel.Settings = GetSettings(args);
                indexModel.Taxonomy = GetTaxonomy(args);
                // If a reply
                var postParent = GetParent(args);
                if (postParent != null && postParent.ParentId != null)
                {
                    indexModel.ParentId = postParent.ParentId;

                    indexModel.ParentIds = postParent.ParentIdItems != null && postParent.ParentIdItems.Any()
                        ? postParent.ParentIdItems.ToList()
                        : null;

                    //If the user has voted on the parent, get the action item
                    var action = await this.GetUserParentAction(indexModel);
                    if(action != null)
                    {
                        if (action.Agree.GetValueOrDefault())
                        {
                            indexModel.ParentVote = "Agree";
                        }
                        if (action.Disagree.GetValueOrDefault())
                        {
                            indexModel.ParentVote = "Disagree";
                        }
                    }
                    //await UpdateParentDocument(postParent, indexModel, args);
                }
                await _elasticService.Index(indexModel);
                await SendWebsocketUpdate(args);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ContentPostActionsDocument> GetUserParentAction(ContentPostDocument post)
        {
            var query = new QueryDescriptor<ContentPostActionsDocument>();
            if (post.ParentId != null && post.UserId != null)
            {
                query.MatchAll();
                query
                    .Term(x => x.UserId.Suffix("keyword"), post.UserId)
                    .Term(x => x.ContentPostId.Suffix("keyword"), post.ParentId)
                ;
            }
            var results = await _elasticPostActionsService.Search<ContentPostActionsDocument>(query, 0, 1);

            //Get the action associated with the reply
            if (!results.IsSuccess() || results.Documents == null || !results.Documents.Any())
            {
                return null;
            }
            return results.Documents?.FirstOrDefault();
        }

        public async Task SendWebsocketUpdate(IPipelineArgs<ContentPostEntity> args)
        {
            ContentPostDocument doc;
            var response = await _elasticService.GetDocument<ContentPostDocument>(args.Value.Id.ToString());
            doc = response.Source;
            // If its a reply to own thread by the user, send the parent as the update instead
            if (doc.PostType == ContentPostType.Page)
            {
                var parentResponse = await _elasticService.GetDocument<ContentPostDocument>(doc.ParentId.ToString());
                doc = parentResponse.Source;
            }
            var payload = new WebsocketMessage<ContentPostDocument>()
            {
                Message = "Create successful",
                Code = 200,
                Data = doc,
                UpdateType = DataRequestCompleteType.Created,
                Group = "Post",
                Alert = true
            };
            await _messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        } 
    }
}
