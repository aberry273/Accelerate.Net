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

namespace Accelerate.Features.Content.Pipelines.Channels
{
    public class ContentChannelDeletedPipeline : DataDeleteEventPipeline<ContentChannelEntity>
    {
        IHubContext<BaseHub<ContentChannelDocument>, IBaseHubClient<WebsocketMessage<ContentChannelDocument>>> _messageHub;
        IElasticService<ContentChannelDocument> _elasticService;
        IEntityService<ContentChannelEntity> _entityService;
        public ContentChannelDeletedPipeline(
            IElasticService<ContentChannelDocument> elasticService,
            IEntityService<ContentChannelEntity> entityService,
            IHubContext<BaseHub<ContentChannelDocument>, IBaseHubClient<WebsocketMessage<ContentChannelDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentChannelEntity>>()
            {
                DeleteDocument,
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentChannelEntity> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<ContentChannelEntity>(args.Value.Id.ToString());

            var docArgs = new PipelineArgs<ContentChannelDocument>()
            {
                Value = new ContentChannelDocument()
                {
                    Id = args.Value.Id,
                }
            };
        }
    }
}
