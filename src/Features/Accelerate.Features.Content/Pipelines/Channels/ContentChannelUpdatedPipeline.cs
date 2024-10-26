using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
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
using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Content.Hydrators;

namespace Accelerate.Features.Content.Pipelines.Channels
{
    public class ContentChannelUpdatedPipeline : DataUpdateEventPipeline<ContentChannelEntity>
    {
        IHubContext<BaseHub<ContentChannelDocument>, IBaseHubClient<WebsocketMessage<ContentChannelDocument>>> _messageHub;
        IElasticService<ContentChannelDocument> _elasticService;
        IEntityService<ContentChannelEntity> _entityService;
        public ContentChannelUpdatedPipeline(
            IElasticService<ContentChannelDocument> elasticService,
            IEntityService<ContentChannelEntity> entityService,
            IHubContext<BaseHub<ContentChannelDocument>, IBaseHubClient<WebsocketMessage<ContentChannelDocument>>> messageHub)
        {
            _messageHub = messageHub;
            _elasticService = elasticService;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentChannelEntity>>()
            {
                IndexDocument,
            };
            _processors = new List<PipelineProcessor<ContentChannelEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentChannelEntity> args)
        {
            var indexModel = new ContentChannelDocument();
            args.Value.Hydrate(indexModel);
            var result = await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
             
            args.Params["IndexedModel"] = indexModel; 
        }
    }
}
