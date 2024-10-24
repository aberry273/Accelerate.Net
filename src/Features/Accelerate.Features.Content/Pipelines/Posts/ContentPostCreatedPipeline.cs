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
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Features.Content.Hydrators;
using System.Reflection.Metadata;
using Accelerate.Features.Content.Services;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostCreatedPipeline : DataCreateEventPipeline<ContentPostEntity>
    {
        private UserManager<AccountUser> _userManager;
        IContentPostService _contentPostService;
        IContentViewSearchService _contentViewSearchService;
        IElasticService<AccountUserDocument> _accountElasticService;
        IElasticService<ContentPostDocument> _elasticService;
        IElasticService<ContentPostActionsDocument> _elasticPostActionsService;
        IElasticService<ContentPostActionsSummaryDocument> _elasticPostActionsSummaryService;
        IEntityService<ContentPostQuoteEntity> _quoteService;
        IEntityService<ContentChannelEntity> _channelService;
        IEntityService<MediaBlobEntity> _mediaService;
        IEntityService<ContentPostMediaEntity> _mediaPostService;
        IHubContext<BaseHub<ContentPostViewDocument>, IBaseHubClient<WebsocketMessage<ContentPostViewDocument>>> _messageHub;
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IEntityService<ContentPostEntity> _entityService;
        IEntityService<ContentPostActionsSummaryEntity> _entityActionsSummaryService;
        IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> _pipelineActivityService;
        public ContentPostCreatedPipeline(
            IContentPostService contentPostService,
            UserManager<AccountUser> userManager,
            IContentViewSearchService contentViewSearchService,
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
            IHubContext<BaseHub<ContentPostViewDocument>, IBaseHubClient<WebsocketMessage<ContentPostViewDocument>>> messageHub,
            IElasticService<AccountUserDocument> accountElasticService)
        {
            _contentPostService = contentPostService;
            _elasticPostActionsService = elasticPostActionsService;
            _entityActionsSummaryService = entityActionsSummaryService;
            _contentViewSearchService = contentViewSearchService;
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

        private List<ContentPostContentMediaSubdocument> GetMedia(IPipelineArgs<ContentPostEntity> args)
        {
            var mediaLinks = _mediaPostService
                .Find(x => x.ContentPostId == args.Value.Id)
                .Select(x => x.MediaBlobId)
                .ToList();
            var media = _mediaService.Find(x => mediaLinks.Contains(x.Id));
            var mediaItems = media.Select(x => new ContentPostContentMediaSubdocument(){
                Src = x.FilePath,
                Type = Enum.GetName(x.Type),
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();
            return mediaItems;
        }
        private ContentPostTaxonomySubdocument GetTaxonomy(IPipelineArgs<ContentPostEntity> args)
        {
            var taxonomy = _contentPostService.GetTaxonomy(args.Value.Id);
            var channel = GetChannel(args);

            return new ContentPostTaxonomySubdocument()
            {
                Category = taxonomy?.Category,
                Tags = taxonomy?.TagItems?.ToList(),
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
        private async Task<ContentPostUserProfileSubdocument> GetBasicUserProfile(IPipelineArgs<ContentPostEntity> args)
        {
            if(args.Value.UserId == Guid.Empty)
            {
                return new ContentPostUserProfileSubdocument()
                {
                    Username = "Unknown"
                };
            }
            var user = await _userManager.FindByIdAsync(args.Value.UserId.ToString());
            if (user == null)
            {
                return new ContentPostUserProfileSubdocument()
                {
                    Username = "Unknown"
                };
            }

            return new ContentPostUserProfileSubdocument()
            {
                Username = user.UserName,
            };
        }
        private ContentPostLinkSubdocument GetLinkDocument(IPipelineArgs<ContentPostEntity> args)
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
        private async Task<List<ContentPostQuoteSubdocument>> GetQuotes(IPipelineArgs<ContentPostEntity> args)
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
        private async Task<ContentPostRelatedPostsSubdocument> GetRelatedDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var model = new ContentPostRelatedPostsSubdocument();
            // Quotes
            model.Quotes = await this.GetQuotes(args);
            // Parents
            var parent = GetParent(args);
            model.ParentId = parent?.ParentId;
            model.ParentIds = parent?.ParentIdItems?.ToList();
            // Channel
            var channel = GetChannel(args);
            if(channel != null)
            {
                model.ChannelId = channel?.Id;
            }
            // Chat
            var chat = GetChat(args);
            if (chat != null)
            {
                model.ChatId = chat?.Id;
            }
            // List
            var list = GetList(args);
            if (list != null)
            {
                model.ListId = list?.Id;
            }
            return model;
        }
        private ContentListEntity? GetList(IPipelineArgs<ContentPostEntity> args)
        {
            return _contentPostService.GetPostList(args.Value);
        }
        private ContentChatEntity? GetChat(IPipelineArgs<ContentPostEntity> args)
        {
            return _contentPostService.GetPostChat(args.Value);
        }
        private ContentChannelEntity? GetChannel(IPipelineArgs<ContentPostEntity> args)
        {
            return _contentPostService.GetPostChannel(args.Value);
        }
        private async Task<ContentPostUserProfileSubdocument> GetUserDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var result = await _accountElasticService.GetDocument<AccountUserDocument>(args.Value.UserId.ToString());
            var doc = result?.Source;

            var model = new ContentPostUserProfileSubdocument();
            doc?.Hydrate(model);
            return model;
        }
        private async Task<ContentPostMetricsSubdocument> GetMetricsDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var model = new ContentPostMetricsSubdocument();
            return model;
        }
        private async Task<ContentPostContentSubdocument> GetContentDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var model = new ContentPostContentSubdocument();
            model.Text = args.Value.Text;
            model.Formats = args.Value.FormatItems;
            model.Media = this.GetMedia(args);
            model.Date = args.Value.UpdatedOn.ToString();
            return model;
        }

        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostEntity> args)
        {
            try
            {
                var indexModel = new ContentPostDocument();

                // skip indexing but send
                indexModel.Profile = await GetBasicUserProfile(args);
                indexModel.Taxonomy = GetTaxonomy(args);
                indexModel.Content = await this.GetContentDocument(args);
                indexModel.Related = await this.GetRelatedDocument(args);
                indexModel.Metrics = await this.GetMetricsDocument(args);
                indexModel.Link = this.GetLinkDocument(args);

                args.Value.Hydrate(indexModel);
                
                await _elasticService.Index(indexModel);
                await SendWebsocketUpdate(indexModel);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        } 
        public async Task<ContentPostActionsDocument> GetUserParentAction(ContentPostDocument post)
        {
            var query = new QueryDescriptor<ContentPostActionsDocument>();
            /*
            if (post.ParentId != null && post.UserId != null)
            {
                query.MatchAll();
                query
                    .Term(x => x.UserId.Suffix("keyword"), post.UserId)
                    .Term(x => x.ContentPostId.Suffix("keyword"), post.ParentId)
                ;
            }
            */
            var results = await _elasticPostActionsService.Search<ContentPostActionsDocument>(query, 0, 1);

            //Get the action associated with the reply
            if (!results.IsSuccess() || results.Documents == null || !results.Documents.Any())
            {
                return null;
            }
            return results.Documents?.FirstOrDefault();
        }

        public async Task SendWebsocketUpdate(ContentPostDocument doc)
        {
            var model = await _contentViewSearchService.UpdatePostDocument(doc.UserId, doc);
            
            var payload = new WebsocketMessage<ContentPostViewDocument>()
            {
                Message = "Create successful",
                Code = 200,
                Data = model,
                UpdateType = DataRequestCompleteType.Created,
                Group = "Post",
                Alert = true
            };
            await _messageHub.Clients.All.SendMessage(doc.UserId.ToString(), payload);
        } 
    }
}
