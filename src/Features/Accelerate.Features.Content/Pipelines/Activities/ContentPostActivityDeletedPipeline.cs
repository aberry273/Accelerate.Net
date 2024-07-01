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
using Accelerate.Foundations.Content.Hydrators;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Activities
{
    public class ContentPostActivityDeletedPipeline : DataDeleteEventPipeline<ContentPostActivityEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostActivityDocument>, IBaseHubClient<WebsocketMessage<ContentPostActivityDocument>>> _messageHub;
        IElasticService<ContentPostActivityDocument> _elasticService;
        IEntityService<ContentPostActivityEntity> _entityService;
        IElasticService<ContentPostDocument> _elasticPostService;
        public ContentPostActivityDeletedPipeline(
            IElasticService<ContentPostDocument> elasticPostService,
            IElasticService<ContentPostActivityDocument> elasticService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostActivityEntity> entityService,
            IHubContext<BaseHub<ContentPostActivityDocument>, IBaseHubClient<WebsocketMessage<ContentPostActivityDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActivityEntity>>()
            {
                DeleteDocument,
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentPostActivityEntity> args)
        {
            var indexModel = new ContentPostActivityDocument();
            args.Value.Hydrate(indexModel);
            var docArgs = new PipelineArgs<ContentPostActivityDocument>() { Value = indexModel };
            await ContentPostActivityUtilities.SendWebsocketActivityUpdate(_messageHub, docArgs, args.Value.Message, DataRequestCompleteType.Deleted);
        }
    }
}
