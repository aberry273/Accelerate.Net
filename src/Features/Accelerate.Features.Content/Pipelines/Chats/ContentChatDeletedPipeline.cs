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

namespace Accelerate.Features.Content.Pipelines.Chats
{
    public class ContentChatDeletedPipeline : DataDeleteEventPipeline<ContentChatEntity>
    {
        IHubContext<BaseHub<ContentChatDocument>, IBaseHubClient<WebsocketMessage<ContentChatDocument>>> _messageHub;
        IElasticService<ContentChatDocument> _elasticService;
        IEntityService<ContentChatEntity> _entityService;
        public ContentChatDeletedPipeline(
            IElasticService<ContentChatDocument> elasticService,
            IEntityService<ContentChatEntity> entityService,
            IHubContext<BaseHub<ContentChatDocument>, IBaseHubClient<WebsocketMessage<ContentChatDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentChatEntity>>()
            {
                DeleteDocument,
                SendWebsocketUpdate
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentChatEntity> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<ContentChatEntity>(args.Value.Id.ToString());

            var docArgs = new PipelineArgs<ContentChatDocument>()
            {
                Value = new ContentChatDocument()
                {
                    Id = args.Value.Id,
                }
            };
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentChatEntity> args)
        {
            var userId = args.Value.UserId.ToString();
            var id = args.Value.Id;

            // Get doc, sned parent ID if its a self rpely
            var payload = new WebsocketMessage<ContentChatDocument>()
            {
                Message = "Delete successful",
                Code = 200,
                Data = new ContentChatDocument()
                {
                    Id = id,
                },
                UpdateType = DataRequestCompleteType.Deleted,
                Group = "Chat",
                Alert = true
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(userId);
            await _messageHub.Clients.Clients(userConnections).SendMessage(userId, payload);
        }

    }
}
