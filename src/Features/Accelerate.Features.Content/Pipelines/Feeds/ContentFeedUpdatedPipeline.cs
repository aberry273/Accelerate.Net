using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
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

namespace Accelerate.Features.Content.Pipelines.Feeds
{
    public class ContentFeedUpdatedPipeline : DataUpdateEventPipeline<ContentFeedEntity>
    {
        IHubContext<BaseHub<ContentFeedDocument>, IBaseHubClient<WebsocketMessage<ContentFeedDocument>>> _messageHub;
        IElasticService<ContentFeedDocument> _elasticService;
        IEntityService<ContentFeedEntity> _entityService;
        public ContentFeedUpdatedPipeline(
            IElasticService<ContentFeedDocument> elasticService,
            IEntityService<ContentFeedEntity> entityService,
            IHubContext<BaseHub<ContentFeedDocument>, IBaseHubClient<WebsocketMessage<ContentFeedDocument>>> messageHub)
        {
            _messageHub = messageHub;
            _elasticService = elasticService;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentFeedEntity>>()
            {
                IndexDocument,
                SendWebsocketUpdate,
            };
            _processors = new List<PipelineProcessor<ContentFeedEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentFeedEntity> args)
        {
            var indexModel = new ContentFeedDocument();
            args.Value.Hydrate(indexModel);
            var result = await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
             
            args.Params["IndexedModel"] = indexModel;
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentFeedEntity> args)
        {
            var doc = await _elasticService.GetDocument<ContentFeedDocument>(args.Value.Id.ToString());
            var payload = new WebsocketMessage<ContentFeedDocument>()
            {
                Message = "Update successful",
                Code = 200,
                Data = doc.Source,
                UpdateType = DataRequestCompleteType.Updated,
                Group = "Feed",
                Alert = true
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(args.Value.UserId.ToString());
            await _messageHub.Clients.Clients(userConnections).SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}
