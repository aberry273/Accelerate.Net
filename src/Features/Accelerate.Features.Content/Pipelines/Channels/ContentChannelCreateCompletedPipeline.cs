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
    public class ContentChannelCreateCompletedPipeline : DataCreateCompletedEventPipeline<ContentChannelEntity>
    {
        IHubContext<BaseHub<ContentChannelDocument>, IBaseHubClient<WebsocketMessage<ContentChannelDocument>>> _messageHub;
        IElasticService<ContentChannelDocument> _elasticService;
        public ContentChannelCreateCompletedPipeline(
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

        // ASYNC PROCESSORS
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentChannelEntity> args)
        {
            ContentChannelDocument doc;
            var response = await _elasticService.GetDocument<ContentChannelDocument>(args.Value.Id.ToString());
            doc = response.Source;
             
            var payload = new WebsocketMessage<ContentChannelDocument>()
            {
                Message = "Create successful",
                Code = 200,
                Data = doc,
                UpdateType = DataRequestCompleteType.Created,
                Group = "Channel",
                Alert = true
            };
            await _messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
        // SYNC PROCESSORS
    }
}
