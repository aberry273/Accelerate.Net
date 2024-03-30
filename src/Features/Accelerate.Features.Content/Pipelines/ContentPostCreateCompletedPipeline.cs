﻿using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines
{
    public class ContentPostCreateCompletedPipeline : DataCreateCompletedEventPipeline<ContentPostEntity>
    {
        IHubContext<BaseHub<ContentPostEntity>, IBaseHubClient<WebsocketMessage<ContentPostEntity>>> _messageHub;
        public ContentPostCreateCompletedPipeline(
            IHubContext<BaseHub<ContentPostEntity>, IBaseHubClient<WebsocketMessage<ContentPostEntity>>> messageHub)
        {
            _messageHub = messageHub;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                SendWebsocketUpdate
            };
            _processors = new List<PipelineProcessor<ContentPostEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentPostEntity> args)
        {
            var payload = new WebsocketMessage<ContentPostEntity>()
            {
                Message = "Create successful",
                Code = 200,
                Data = args.Value,
                UpdateType = DataRequestCompleteType.Created,
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(args.Value.UserId.ToString());
            await _messageHub.Clients.Clients(userConnections).SendMessage(args.Value.UserId.ToString(), payload);
        }
        // SYNC PROCESSORS
    }
}
