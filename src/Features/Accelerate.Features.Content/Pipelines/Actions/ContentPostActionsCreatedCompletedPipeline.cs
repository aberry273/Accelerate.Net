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

namespace Accelerate.Features.Content.Pipelines.Actions
{
    public class ContentPostActionsCreatedCompletedPipeline : DataCreateCompletedEventPipeline<ContentPostActionsEntity>
    {
        IHubContext<BaseHub<ContentPostActionsDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsDocument>>> _messageHub;
        IElasticService<ContentPostActionsDocument> _elasticService;
        public ContentPostActionsCreatedCompletedPipeline(
            IElasticService<ContentPostActionsDocument> elasticService,
            IHubContext<BaseHub<ContentPostActionsDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActionsEntity>>()
            {
                SendWebsocketUpdate
            };
            _processors = new List<PipelineProcessor<ContentPostActionsEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS

        // ASYNC PROCESSORS
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentPostActionsEntity> args)
        {
            ContentPostActionsDocument doc;
            var response = await _elasticService.GetDocument<ContentPostActionsDocument>(args.Value.Id.ToString());
            doc = response.Source;
           
            var payload = new WebsocketMessage<ContentPostActionsDocument>()
            {
                Message = "Create successful",
                Code = 200,
                Data = doc,
                UpdateType = DataRequestCompleteType.Created,
                Group = "PostActions",
                Alert = true
            };
            await _messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
        // SYNC PROCESSORS
    }
}
