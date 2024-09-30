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

namespace Accelerate.Features.Content.Pipelines.Feeds
{
    public class ContentFeedDeletedPipeline : DataDeleteEventPipeline<ContentFeedEntity>
    {
        IHubContext<BaseHub<ContentFeedDocument>, IBaseHubClient<WebsocketMessage<ContentFeedDocument>>> _messageHub;
        IElasticService<ContentFeedDocument> _elasticService;
        IEntityService<ContentFeedEntity> _entityService;
        public ContentFeedDeletedPipeline(
            IElasticService<ContentFeedDocument> elasticService,
            IEntityService<ContentFeedEntity> entityService,
            IHubContext<BaseHub<ContentFeedDocument>, IBaseHubClient<WebsocketMessage<ContentFeedDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentFeedEntity>>()
            {
                DeleteDocument,
                SendWebsocketUpdate
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentFeedEntity> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<ContentFeedEntity>(args.Value.Id.ToString());

            var docArgs = new PipelineArgs<ContentFeedDocument>()
            {
                Value = new ContentFeedDocument()
                {
                    Id = args.Value.Id,
                }
            };
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentFeedEntity> args)
        {
            var userId = args.Value.UserId.ToString();
            var id = args.Value.Id;

            // Get doc, sned parent ID if its a self rpely
            var payload = new WebsocketMessage<ContentFeedDocument>()
            {
                Message = "Delete successful",
                Code = 200,
                Data = new ContentFeedDocument()
                {
                    Id = id,
                },
                UpdateType = DataRequestCompleteType.Deleted,
                Group = "Feed",
                Alert = true
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(userId);
            await _messageHub.Clients.Clients(userConnections).SendMessage(userId, payload);
        }

    }
}
