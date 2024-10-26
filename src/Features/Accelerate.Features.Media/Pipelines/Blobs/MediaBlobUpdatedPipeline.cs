
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
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
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Media.Hydrators;
using Accelerate.Foundations.Media.Services;
using Accelerate.Foundations.Media.Models.Entities;
using Accelerate.Features.Media.Pipelines.Blobs;

namespace Accelerate.Features.Media.Pipelines.Channels
{
    public class MediaBlobUpdatedPipeline : DataUpdateEventPipeline<MediaBlobEntity>
    {
        IHubContext<BaseHub<MediaBlobDocument>, IBaseHubClient<WebsocketMessage<MediaBlobDocument>>> _messageHub;
        IElasticService<MediaBlobDocument> _elasticService;
        IEntityService<MediaBlobEntity> _entityService;
        public MediaBlobUpdatedPipeline(
            IElasticService<MediaBlobDocument> elasticService,
            IEntityService<MediaBlobEntity> entityService,
            IHubContext<BaseHub<MediaBlobDocument>, IBaseHubClient<WebsocketMessage<MediaBlobDocument>>> messageHub)
        {
            _messageHub = messageHub;
            _elasticService = elasticService;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<MediaBlobEntity>>()
            {
                IndexDocument,
            };
            _processors = new List<PipelineProcessor<MediaBlobEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<MediaBlobEntity> args)
        {
            var indexModel = new MediaBlobDocument();
            args.Value.Hydrate(indexModel);
            var result = await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
            if (result.IsValidResponse)
            {
                var docArgs = new PipelineArgs<MediaBlobDocument>()
                {
                    Value = indexModel
                };
                await Utilities.SendWebsocketUpdate(_messageHub, docArgs, "Update media successful", DataRequestCompleteType.Updated);
            }
            args.Params["IndexedModel"] = indexModel; 
        }
    }
}
