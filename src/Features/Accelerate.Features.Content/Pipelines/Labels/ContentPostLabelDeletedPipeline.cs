﻿
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

namespace Accelerate.Features.Content.Pipelines.Labels
{
    public class ContentPostLabelDeletedPipeline : DataDeleteEventPipeline<ContentPostLabelEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostLabelDocument>, IBaseHubClient<WebsocketMessage<ContentPostLabelDocument>>> _messageHub;
        IElasticService<ContentPostLabelDocument> _elasticService;
        IEntityService<ContentPostLabelEntity> _entityService;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> _pipelineActivityService;

        public ContentPostLabelDeletedPipeline(
            IElasticService<ContentPostDocument> elasticPostService,
            IElasticService<ContentPostLabelDocument> elasticService,
            IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> pipelineActivityService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostLabelEntity> entityService,
            IHubContext<BaseHub<ContentPostLabelDocument>, IBaseHubClient<WebsocketMessage<ContentPostLabelDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            _pipelineActivityService = pipelineActivityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostLabelEntity>>()
            {
                DeleteDocument, 
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentPostLabelEntity> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<ContentPostLabelEntity>(args.Value.Id.ToString());

            var docArgs = new PipelineArgs<ContentPostLabelDocument>()
            {
                Value = new ContentPostLabelDocument()
                {
                    Id = args.Value.Id,
                }
            };
            //await ContentPostLabelUtilities.SendWebsocketLabelUpdate(_messageHub, docArgs, "Delete Action successful", DataRequestCompleteType.Deleted);
        } 
    }
}
