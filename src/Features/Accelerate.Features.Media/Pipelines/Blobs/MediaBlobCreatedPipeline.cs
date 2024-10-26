using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Media.Hydrators;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MSearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR; 
using System;
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Media.Models.Entities;
using static MassTransit.ValidationResultExtensions;
using Accelerate.Features.Media.Pipelines.Blobs;

namespace Accelerate.Features.Media.Pipelines.Channels
{
    public class MediaBlobCreatedPipeline : DataCreateEventPipeline<MediaBlobEntity>
    {
        IHubContext<BaseHub<MediaBlobDocument>, IBaseHubClient<WebsocketMessage<MediaBlobDocument>>> _messageHub;
        IElasticService<MediaBlobDocument> _elasticService;
        IEntityService<MediaBlobEntity> _entityService;
        public MediaBlobCreatedPipeline(
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
            var result = await _elasticService.Index(indexModel);

            if (result.IsValidResponse)
            {
                var docArgs = new PipelineArgs<MediaBlobDocument>()
                {
                    Value = indexModel
                };
                await Utilities.SendWebsocketUpdate(_messageHub, docArgs, "Create media successful", DataRequestCompleteType.Created);
            }
        }
    }
}
