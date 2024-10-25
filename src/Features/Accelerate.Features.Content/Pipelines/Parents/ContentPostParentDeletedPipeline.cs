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

namespace Accelerate.Features.Content.Pipelines.Parents
{
    public class ContentPostParentDeletedPipeline : DataDeleteEventPipeline<ContentPostParentEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostParentDocument>, IBaseHubClient<WebsocketMessage<ContentPostParentDocument>>> _messageHub;
        IElasticService<ContentPostParentDocument> _elasticService;
        IEntityService<ContentPostParentEntity> _entityService;
        IElasticService<ContentPostDocument> _elasticPostService;
        public ContentPostParentDeletedPipeline(
            IElasticService<ContentPostDocument> elasticPostService,
            IElasticService<ContentPostParentDocument> elasticService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostParentEntity> entityService,
            IHubContext<BaseHub<ContentPostParentDocument>, IBaseHubClient<WebsocketMessage<ContentPostParentDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostParentEntity>>()
            {
                DeleteDocument,
               // UpdatePostIndex
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentPostParentEntity> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<ContentPostParentEntity>(args.Value.Id.ToString());

            var docArgs = new PipelineArgs<ContentPostParentDocument>()
            {
                Value = new ContentPostParentDocument()
                {
                    Id = args.Value.Id,
                }
            };
            await ContentPostParentUtilities.SendWebsocketParentUpdate(_messageHub, docArgs, "Delete Action successful", DataRequestCompleteType.Deleted);
        }
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostParentEntity> args)
        {
            // fetch Actions
            var ActionsDoc = ContentPostParentUtilities.GetTotalParents(_entityService, args);
            var fetchResponse = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.ParentdContentPostId.ToString());
            var contentPostDocument = fetchResponse.Source;
            
            contentPostDocument.UpdatedOn = DateTime.Now;
            await _elasticPostService.UpdateDocument(contentPostDocument, args.Value?.ParentdContentPostId.ToString());

            // Send websocket request
            await ContentPostUtilities.SendWebsocketPostUpdate(_messageHubPosts, args.Value?.UserId.ToString(), contentPostDocument, DataRequestCompleteType.Updated);
        } 
    }
}
