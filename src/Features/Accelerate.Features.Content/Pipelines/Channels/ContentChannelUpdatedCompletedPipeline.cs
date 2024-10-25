using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
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
    public class ContentChannelUpdatedCompletedPipeline : DataUpdateCompletedEventPipeline<ContentChannelEntity>
    {
        IHubContext<BaseHub<ContentChannelDocument>, IBaseHubClient<WebsocketMessage<ContentChannelDocument>>> _messageHub;
        IElasticService<ContentChannelDocument> _elasticService;
        public ContentChannelUpdatedCompletedPipeline(
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
            var doc = await _elasticService.GetDocument<ContentChannelDocument>(args.Value.Id.ToString());
            var payload = new WebsocketMessage<ContentChannelDocument>()
            {
                Message = "Update successful",
                Code = 200,
                Data = doc.Source,
                UpdateType = DataRequestCompleteType.Updated,
                Group = "Channel",
                Alert = true
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(args.Value.UserId.ToString());
            await _messageHub.Clients.Clients(userConnections).SendMessage(args.Value.UserId.ToString(), payload);
        }
        // SYNC PROCESSORS
    }
}
