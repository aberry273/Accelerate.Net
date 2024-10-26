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

namespace Accelerate.Features.Content.Pipelines.Chats
{
    public class ContentChatUpdatedPipeline : DataUpdateEventPipeline<ContentChatEntity>
    {
        IHubContext<BaseHub<ContentChatDocument>, IBaseHubClient<WebsocketMessage<ContentChatDocument>>> _messageHub;
        IElasticService<ContentChatDocument> _elasticService;
        IEntityService<ContentChatEntity> _entityService;
        public ContentChatUpdatedPipeline(
            IElasticService<ContentChatDocument> elasticService,
            IEntityService<ContentChatEntity> entityService,
            IHubContext<BaseHub<ContentChatDocument>, IBaseHubClient<WebsocketMessage<ContentChatDocument>>> messageHub)
        {
            _messageHub = messageHub;
            _elasticService = elasticService;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentChatEntity>>()
            {
                IndexDocument,
                SendWebsocketUpdate,
            };
            _processors = new List<PipelineProcessor<ContentChatEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentChatEntity> args)
        {
            var indexModel = new ContentChatDocument();
            args.Value.Hydrate(indexModel);
            var result = await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
             
            args.Params["IndexedModel"] = indexModel;
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentChatEntity> args)
        {
            var doc = await _elasticService.GetDocument<ContentChatDocument>(args.Value.Id.ToString());
            var payload = new WebsocketMessage<ContentChatDocument>()
            {
                Message = "Update successful",
                Code = 200,
                Data = doc.Source,
                UpdateType = DataRequestCompleteType.Updated,
                Group = "Chat",
                Alert = true
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(args.Value.UserId.ToString());
            await _messageHub.Clients.Clients(userConnections).SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}
