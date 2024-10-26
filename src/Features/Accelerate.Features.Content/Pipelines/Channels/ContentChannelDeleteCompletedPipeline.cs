using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Channels
{
    public class ContentChannelDeleteCompletedPipeline : DataDeleteCompletedEventPipeline<ContentChannelEntity>
    {
        IElasticService<ContentChannelDocument> _elasticService;
        IHubContext<BaseHub<ContentChannelDocument>, IBaseHubClient<WebsocketMessage<ContentChannelDocument>>> _messageHub;
        public ContentChannelDeleteCompletedPipeline(
            IElasticService<ContentChannelDocument> elasticService,
            IHubContext<BaseHub<ContentChannelDocument>, IBaseHubClient<WebsocketMessage<ContentChannelDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentChannelEntity>>()
            {
                SendWebsocketUpdate
            };
            _processors = new List<PipelineProcessor<ContentChannelEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentChannelEntity> args)
        {
            var userId = args.Value.UserId.ToString();
            var id = args.Value.Id;

            // Get doc, sned parent ID if its a self rpely
            var payload = new WebsocketMessage<ContentChannelDocument>()
            {
                Message = "Delete successful",
                Code = 200,
                Data = new ContentChannelDocument()
                {
                    Id = id,
                },
                UpdateType = DataRequestCompleteType.Deleted,
                Group = "Channel",
                Alert = true
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(userId);
            await _messageHub.Clients.Clients(userConnections).SendMessage(userId, payload);
        }

        // SYNC PROCESSORS
    }
}
