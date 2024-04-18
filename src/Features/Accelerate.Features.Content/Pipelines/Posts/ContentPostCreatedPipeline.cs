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
using Accelerate.Features.Content.Pipelines.Reviews;
using Accelerate.Foundations.Content.Hydrators;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostCreatedPipeline : DataCreateEventPipeline<ContentPostEntity>
    {
        IElasticService<AccountUserDocument> _accountElasticService;
        IElasticService<ContentPostDocument> _elasticService;
        IEntityService<ContentPostQuoteEntity> _quoteService;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHub;
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IEntityService<ContentPostEntity> _entityService;
        public ContentPostCreatedPipeline(
            IElasticService<ContentPostDocument> elasticService,
            IEntityService<ContentPostEntity> entityService,
            IEntityService<ContentPostQuoteEntity> quoteService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHub,
            IElasticService<AccountUserDocument> accountElasticService)
        {
            _entityService = entityService;
            _elasticService = elasticService;
            _messageHub = messageHub;
            _quoteService = quoteService;
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
        private List<string> GetQuoteIds(IPipelineArgs<ContentPostEntity> args)
        {
            var quotes = _quoteService.Find(x => x.QuoterContentPostId == args.Value.Id);
            return quotes.Select(x => x.Value).ToList();
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var user = await _accountElasticService.GetDocument<AccountUserDocument>(args.Value.UserId.GetValueOrDefault().ToString());
            var indexModel = new ContentPostDocument();
            args.Value.Hydrate(indexModel, user?.Source?.Username);
            
            indexModel.QuoteIds = GetQuoteIds(args);
            // If a reply
            if (args.Value.ParentId != null)
            {
                var parentResponse = await _elasticService.GetDocument<ContentPostDocument>(args.Value.ParentId.ToString());
                var parentDoc = parentResponse.Source;
                await UpdateParentDocument(parentDoc, indexModel, args);

                var parentIdThread = parentDoc.ParentIds ?? new List<Guid>();
                parentIdThread.Add(parentDoc.Id);
                indexModel.ParentIds = parentIdThread;
            }
            await _elasticService.Index(indexModel);
        }

        private async Task UpdateParentDocument(ContentPostDocument parentDoc, ContentPostDocument childDoc, IPipelineArgs<ContentPostEntity> args)
        {
            if (parentDoc == null) return;
            // Update reply count
            var reviewsDoc = ContentPostUtilities.GetReplies(_entityService, args);
            parentDoc.Replies = reviewsDoc?.Replies ?? 0;
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
