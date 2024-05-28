using Accelerate.Features.Content.EventBus;
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

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostCreatedPipeline : DataCreateEventPipeline<ContentPostEntity>
    {
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
            IElasticService<ContentPostDocument> elasticService,
            IEntityService<ContentChannelEntity> channelService,
            IEntityService<ContentPostEntity> entityService,
            IEntityService<ContentPostQuoteEntity> quoteService,
            IEntityService<MediaBlobEntity> mediaService,
            IEntityService<ContentPostMediaEntity> mediaPostService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHub,
            IElasticService<AccountUserDocument> accountElasticService)
        {
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
        private List<string> GetQuoteIds(IPipelineArgs<ContentPostEntity> args)
        {
            var quotes = _quoteService.Find(x => x.QuoterContentPostId == args.Value.Id);
            var quotedPosts = quotes.Select(x => x.Value).ToList();
            // add the quoter to the list of quotes so it is returned while searching
            quotedPosts.Add(Foundations.Common.Extensions.GuidExtensions.ShortenBase64(args.Value.ThreadId));
            return quotedPosts;
        }
        private string? GetChannelName(IPipelineArgs<ContentPostEntity> args)
        {
            if (string.IsNullOrEmpty(args.Value.TargetChannel)) return null;
            var channel = _channelService.Get(Guid.Parse(args.Value.TargetChannel));
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
                indexModel.QuoteIds = GetQuoteIds(args);
                indexModel.Media = GetMedia(args);
                // If a reply
                
                if (args.Value.ParentId != null)
                {
                    var parentResponse = await _elasticService.GetDocument<ContentPostDocument>(args.Value.ParentId.ToString());
                    var parentDoc = parentResponse.Source;
                    await UpdateParentDocument(parentDoc, indexModel, args);
                   /*
                    var parentIdThread = parentDoc.ParentIds ?? new List<Guid>();
                    parentIdThread.Add(parentDoc.Id);
                    indexModel.ParentIds = parentIdThread;
                   */
                }
                await _elasticService.Index(indexModel);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task UpdateParentDocument(ContentPostDocument parentDoc, ContentPostDocument childDoc, IPipelineArgs<ContentPostEntity> args)
        {
            if (parentDoc == null) return;
            // Update reply count
            var replies = ContentPostUtilities.GetReplies(_entityService, args);
            parentDoc.Replies = replies ?? 0;
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
