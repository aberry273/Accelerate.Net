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

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostActionsDeletedPipeline : DataDeleteEventPipeline<ContentPostActionsEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostActionsDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsDocument>>> _messageHub;
        IElasticService<ContentPostActionsDocument> _elasticService;
        IEntityService<ContentPostActionsEntity> _entityService;
        IElasticService<ContentPostDocument> _elasticPostService;
        public ContentPostActionsDeletedPipeline(
            IElasticService<ContentPostDocument> elasticPostService,
            IElasticService<ContentPostActionsDocument> elasticService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostActionsEntity> entityService,
            IHubContext<BaseHub<ContentPostActionsDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActionsEntity>>()
            {
                //DeleteDocument,

                //UpdatePostIndex
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentPostActionsEntity> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<ContentPostActionsEntity>(args.Value.Id.ToString());

            var docArgs = new PipelineArgs<ContentPostActionsDocument>()
            {
                Value = new ContentPostActionsDocument()
                {
                    Id = args.Value.Id,
                }
            };
            await ContentPostActionUtilities.SendWebsocketActionUpdate(_messageHub, docArgs, "Delete Action successful", DataRequestCompleteType.Deleted);
        }
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostActionsEntity> args)
        {
            // fetch Actions
            var ActionsDoc = ContentPostActionUtilities.GetActions(_entityService, args);
            var fetchResponse = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.ContentPostId.ToString());
            var contentPostDocument = fetchResponse.Source;
            //contentPostDocument.ActionsTotals = ActionsDoc;
            contentPostDocument.UpdatedOn = DateTime.Now;
            await _elasticPostService.UpdateDocument(contentPostDocument, args.Value?.ContentPostId.ToString());

            // Send websocket request
            await ContentPostUtilities.SendWebsocketPostUpdate(_messageHubPosts, args.Value?.UserId.ToString(), contentPostDocument, DataRequestCompleteType.Updated);
        } 
    }
}
