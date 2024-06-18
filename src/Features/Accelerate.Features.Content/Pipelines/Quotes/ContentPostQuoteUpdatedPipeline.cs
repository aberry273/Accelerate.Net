using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
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
using Elastic.Clients.Elasticsearch.Core.MSearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using MassTransit.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Content.Hydrators;

namespace Accelerate.Features.Content.Pipelines.Quotes
{
    public class ContentPostQuoteUpdatedPipeline : DataUpdateEventPipeline<ContentPostQuoteEntity>
    {
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostQuoteDocument>, IBaseHubClient<WebsocketMessage<ContentPostQuoteDocument>>> _messageHub;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityService<ContentPostQuoteEntity> _entityService;
        IElasticService<ContentPostQuoteDocument> _elasticService;
        public ContentPostQuoteUpdatedPipeline(
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostQuoteDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IEntityService<ContentPostQuoteEntity> entityService,
            IHubContext<BaseHub<ContentPostQuoteDocument>, IBaseHubClient<WebsocketMessage<ContentPostQuoteDocument>>> messageHub,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts)
        {
            _messageHub = messageHub;
            _messageHubPosts = messageHubPosts;
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _entityService = entityService;
            _publishEndpoint = publishEndpoint;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostQuoteEntity>>()
            {
                IndexDocument,
              //  UpdatePostIndex,
               // PublishPostUpdatedMessage
            };
            _processors = new List<PipelineProcessor<ContentPostQuoteEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostQuoteEntity> args)
        {
            var indexModel = new ContentPostQuoteDocument();
            args.Value.Hydrate(indexModel);
            var result = await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
            // Run directly in same function as Actions are simple types and will not be extended via pipelines
            if(result.IsValidResponse)
            {
                var docArgs = new PipelineArgs<ContentPostQuoteDocument>()
                {
                    Value = indexModel
                };
                await ContentPostQuoteUtilities.SendWebsocketQuoteUpdate(_messageHub, docArgs, "Update Action successful", DataRequestCompleteType.Updated);
            }
            args.Params["IndexedModel"] = indexModel; 
        }
        /// <summary>
        /// TODO: REFACTOR THIS ONCE POC READY
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostQuoteEntity> args)
        {
            // fetch Actions
            var ActionsDoc = ContentPostQuoteUtilities.GetTotalQuotes(_entityService, args);
            var fetchResponse = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.QuotedContentPostId.ToString());
            var contentPostDocument = fetchResponse.Source; 
     
            contentPostDocument.UpdatedOn = DateTime.Now;
            await _elasticPostService.UpdateDocument(contentPostDocument, args.Value?.QuotedContentPostId.ToString());

            // Send websocket request
            await ContentPostQuoteUtilities.SendWebsocketPostUpdate(_messageHubPosts ,args.Value?.UserId.ToString(), contentPostDocument, DataRequestCompleteType.Updated);
        }
    }
}
