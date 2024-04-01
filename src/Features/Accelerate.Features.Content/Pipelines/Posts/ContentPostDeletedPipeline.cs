using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostDeletedPipeline : DataDeleteEventPipeline<ContentPostEntity>
    {
        IElasticService<ContentPostDocument> _elasticService;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHub;
        public ContentPostDeletedPipeline(
            IElasticService<ContentPostDocument> elasticService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                DeleteDocument,
                SendWebsocketUpdate
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentPostEntity> args)
        {
            await _elasticService.DeleteDocument<ContentPostEntity>(args.Value.Id.ToString());
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentPostEntity> args)
        {
            //var doc = await _elasticService.GetDocument<ContentPostDocument>(args.Value.Id.ToString());
            var payload = new WebsocketMessage<ContentPostDocument>()
            {
                Message = "Delete successful",
                Code = 200,
                Data = new ContentPostDocument()
                {
                    Id = args.Value.Id,
                },
                UpdateType = DataRequestCompleteType.Deleted,
                Group = "Post"
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(args.Value.UserId.ToString());
            await _messageHub.Clients.Clients(userConnections).SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}
