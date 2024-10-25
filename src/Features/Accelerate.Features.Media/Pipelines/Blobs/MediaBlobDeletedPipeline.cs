
using Accelerate.Features.Media.Pipelines.Blobs;
using Accelerate.Foundations.Common.Pipelines; 
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Media.Models.Entities;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.SignalR;
using static MassTransit.ValidationResultExtensions;

namespace Accelerate.Features.Media.Pipelines.Channels
{
    public class MediaBlobDeletedPipeline : DataDeleteEventPipeline<MediaBlobEntity>
    {
        IHubContext<BaseHub<MediaBlobDocument>, IBaseHubClient<WebsocketMessage<MediaBlobDocument>>> _messageHub;
        IElasticService<MediaBlobDocument> _elasticService;
        IEntityService<MediaBlobEntity> _entityService;
        public MediaBlobDeletedPipeline(
            IElasticService<MediaBlobDocument> elasticService,
            IEntityService<MediaBlobEntity> entityService,
            IHubContext<BaseHub<MediaBlobDocument>, IBaseHubClient<WebsocketMessage<MediaBlobDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _entityService = entityService;
            _messageHub = messageHub;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<MediaBlobEntity>>()
            {
                DeleteDocument,
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<MediaBlobEntity> args)
        {
            var result = await _elasticService.DeleteDocument<MediaBlobEntity>(args.Value.Id.ToString());
            if (result.IsValidResponse)
            {
                var docArgs = new PipelineArgs<MediaBlobDocument>()
                {
                    Value = new MediaBlobDocument()
                    {
                        Id = args.Value.Id,
                    }
                };
                await Utilities.SendWebsocketUpdate(_messageHub, docArgs, "Delete media successful", DataRequestCompleteType.Deleted);
            }
        }
    }
}
