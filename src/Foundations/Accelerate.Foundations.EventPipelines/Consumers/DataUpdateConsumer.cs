using Accelerate.Foundations.EventPipelines.EventBus;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Contracts;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Accelerate.Foundations.EventPipelines.Consumers
{

    public class DataUpdateConsumer<T, B> : IConsumer<UpdateDataContract<T>> where B : IDataBus<T>
    {
        readonly ILogger<DataUpdateConsumer<T, B>> _logger;
        IDataUpdateEventPipeline<T> _pipeline;
        readonly Bind<B, IPublishEndpoint> _publishEndpoint;
        IHubContext<BaseHub<T>, IBaseHubClient<WebsocketMessage<T>>> _messageHub;
        public DataUpdateConsumer(
            IHubContext<BaseHub<T>, IBaseHubClient<WebsocketMessage<T>>> messageHub,
            IDataUpdateEventPipeline<T> pipeline,
            Bind<B, IPublishEndpoint> publishEndpoint,
            ILogger<DataUpdateConsumer<T, B>> logger)
        {
            _logger = logger;
            _pipeline = pipeline;
            _publishEndpoint = publishEndpoint;
            _messageHub = messageHub;
        }

        public async Task Consume(ConsumeContext<UpdateDataContract<T>> context)
        {
            Foundations.Common.Services.StaticLoggingService.Log($"DataUpdatePipeline [Started]]");
            try
            {
                // Run synchronous pipeline first
                _pipeline.Run(context.Message.Data);
                //Run async pipeline second
                await _pipeline.RunAsync(context.Message.Data);
                // Emit complete event
                await _publishEndpoint.Value.Publish(new UpdateCompleteDataContract<T>() {
                    Data = context.Message.Data,
                    Target = context.Message.Target,
                    UserId = context.Message.UserId
                });
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                var payload = new WebsocketMessage<T>()
                {
                    Message = "Update failed",
                    Code = 500,
                    UpdateType = DataRequestCompleteType.Updated
                };
                await _messageHub.Clients.All.SendMessage("user", payload);
            }
            Foundations.Common.Services.StaticLoggingService.Log($"DataUpdatePipeline [Finished]]");
        }
    }
}
