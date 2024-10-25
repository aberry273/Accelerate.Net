using Accelerate.Features.Content.Pipelines.Actions;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Quotes
{
    public class ContentPostQuoteDeletedPipeline : DataDeleteEventPipeline<ContentPostQuoteEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostQuoteDocument>, IBaseHubClient<WebsocketMessage<ContentPostQuoteDocument>>> _messageHub;
        IElasticService<ContentPostQuoteDocument> _elasticService;
        IEntityService<ContentPostQuoteEntity> _entityService;
        IElasticService<ContentPostDocument> _elasticPostService;
        public ContentPostQuoteDeletedPipeline(
            IElasticService<ContentPostDocument> elasticPostService,
            IElasticService<ContentPostQuoteDocument> elasticService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostQuoteEntity> entityService,
            IHubContext<BaseHub<ContentPostQuoteDocument>, IBaseHubClient<WebsocketMessage<ContentPostQuoteDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostQuoteEntity>>()
            {
                DeleteDocument,
               // UpdatePostIndex
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentPostQuoteEntity> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<ContentPostQuoteEntity>(args.Value.Id.ToString());

            var docArgs = new PipelineArgs<ContentPostQuoteDocument>()
            {
                Value = new ContentPostQuoteDocument()
                {
                    Id = args.Value.Id,
                }
            };
            await ContentPostParentUtilities.SendWebsocketQuoteUpdate(_messageHub, docArgs, "Delete Action successful", DataRequestCompleteType.Deleted);
        }
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostQuoteEntity> args)
        {
            // fetch Actions
            var ActionsDoc = ContentPostParentUtilities.GetTotalQuotes(_entityService, args);
            var fetchResponse = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.QuotedContentPostId.ToString());
            var contentPostDocument = fetchResponse.Source;
            
            contentPostDocument.UpdatedOn = DateTime.Now;
            await _elasticPostService.UpdateDocument(contentPostDocument, args.Value?.QuotedContentPostId.ToString());

            // Send websocket request
            await ContentPostUtilities.SendWebsocketPostUpdate(_messageHubPosts, args.Value?.UserId.ToString(), contentPostDocument, DataRequestCompleteType.Updated);
        } 
    }
}
