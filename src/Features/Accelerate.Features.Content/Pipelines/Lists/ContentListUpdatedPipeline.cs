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

namespace Accelerate.Features.Content.Pipelines.Lists
{
    public class ContentListUpdatedPipeline : DataUpdateEventPipeline<ContentListEntity>
    {
        IHubContext<BaseHub<ContentListDocument>, IBaseHubClient<WebsocketMessage<ContentListDocument>>> _messageHub;
        IElasticService<ContentListDocument> _elasticService;
        IEntityService<ContentListEntity> _entityService;
        public ContentListUpdatedPipeline(
            IElasticService<ContentListDocument> elasticService,
            IEntityService<ContentListEntity> entityService,
            IHubContext<BaseHub<ContentListDocument>, IBaseHubClient<WebsocketMessage<ContentListDocument>>> messageHub)
        {
            _messageHub = messageHub;
            _elasticService = elasticService;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentListEntity>>()
            {
                IndexDocument,
                SendWebsocketUpdate,
            };
            _processors = new List<PipelineProcessor<ContentListEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentListEntity> args)
        {
            var indexModel = new ContentListDocument();
            args.Value.Hydrate(indexModel);
            var result = await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
             
            args.Params["IndexedModel"] = indexModel;
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentListEntity> args)
        {
            var doc = await _elasticService.GetDocument<ContentListDocument>(args.Value.Id.ToString());
            var payload = new WebsocketMessage<ContentListDocument>()
            {
                Message = "Update successful",
                Code = 200,
                Data = doc.Source,
                UpdateType = DataRequestCompleteType.Updated,
                Group = "List",
                Alert = true
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(args.Value.UserId.ToString());
            await _messageHub.Clients.Clients(userConnections).SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}
