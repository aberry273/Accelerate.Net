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

namespace Accelerate.Features.Content.Pipelines.Lists
{
    public class ContentListDeletedPipeline : DataDeleteEventPipeline<ContentListEntity>
    {
        IHubContext<BaseHub<ContentListDocument>, IBaseHubClient<WebsocketMessage<ContentListDocument>>> _messageHub;
        IElasticService<ContentListDocument> _elasticService;
        IEntityService<ContentListEntity> _entityService;
        public ContentListDeletedPipeline(
            IElasticService<ContentListDocument> elasticService,
            IEntityService<ContentListEntity> entityService,
            IHubContext<BaseHub<ContentListDocument>, IBaseHubClient<WebsocketMessage<ContentListDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentListEntity>>()
            {
                DeleteDocument,
                SendWebsocketUpdate
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentListEntity> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<ContentListEntity>(args.Value.Id.ToString());

            var docArgs = new PipelineArgs<ContentListDocument>()
            {
                Value = new ContentListDocument()
                {
                    Id = args.Value.Id,
                }
            };
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentListEntity> args)
        {
            var userId = args.Value.UserId.ToString();
            var id = args.Value.Id;

            // Get doc, sned parent ID if its a self rpely
            var payload = new WebsocketMessage<ContentListDocument>()
            {
                Message = "Delete successful",
                Code = 200,
                Data = new ContentListDocument()
                {
                    Id = id,
                },
                UpdateType = DataRequestCompleteType.Deleted,
                Group = "List",
                Alert = true
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(userId);
            await _messageHub.Clients.Clients(userConnections).SendMessage(userId, payload);
        }

    }
}
