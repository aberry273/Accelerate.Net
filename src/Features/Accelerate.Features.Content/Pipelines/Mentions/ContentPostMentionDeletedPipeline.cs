
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.EventPipelines.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Mentions
{
    public class ContentPostMentionDeletedPipeline : DataDeleteEventPipeline<ContentPostMentionEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostMentionDocument>, IBaseHubClient<WebsocketMessage<ContentPostMentionDocument>>> _messageHub;
        //IElasticService<ContentPostMentionDocument> _elasticService;
        IEntityService<ContentPostMentionEntity> _entityService;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> _pipelineActivityService;

        public ContentPostMentionDeletedPipeline(
            IElasticService<ContentPostDocument> elasticPostService,
            //IElasticService<ContentPostMentionDocument> elasticService,
            IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> pipelineActivityService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostMentionEntity> entityService,
            IHubContext<BaseHub<ContentPostMentionDocument>, IBaseHubClient<WebsocketMessage<ContentPostMentionDocument>>> messageHub)
        {
            //_elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            _pipelineActivityService = pipelineActivityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostMentionEntity>>()
            {
                DeleteDocument, 
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentPostMentionEntity> args)
        {
            //var indexResponse = await _elasticService.DeleteDocument<ContentPostMentionEntity>(args.Value.Id.ToString());

            var docArgs = new PipelineArgs<ContentPostMentionDocument>()
            {
                Value = new ContentPostMentionDocument()
                {
                    Id = args.Value.Id,
                }
            };
            await ContentPostMentionUtilities.SendWebsocketMentionUpdate(_messageHub, docArgs, "Delete Action successful", DataRequestCompleteType.Deleted);
        } 
    }
}
