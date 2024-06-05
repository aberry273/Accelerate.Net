using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.EventPipelines.EventBus;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Contracts;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using MassTransit;
using MassTransit.DependencyInjection;
using MassTransit.Transports;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Accelerate.Foundations.EventPipelines.Consumers
{ 
    public class EventListenerConsumer<T, B> : IConsumer<DataContract<T>> where B : IDataBus<T>
    {
        readonly ILogger<EventListenerConsumer<T, B>> _logger;
        readonly Bind<B, IPublishEndpoint> _publishEndpoint;
        public EventListenerConsumer(
            Bind<B, IPublishEndpoint> publishEndpoint,
            ILogger<EventListenerConsumer<T, B>> logger)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<DataContract<T>> context)
        {
            Foundations.Common.Services.StaticLoggingService.Log($"EventListenerConsumer [Started]]");
            try
            {
                await _publishEndpoint.Value.Publish(new DataContract<T>()
                {
                    Data = context.Message.Data,
                    Target = context.Message.Target,
                    UserId = context.Message.UserId
                });
            }
            catch (Exception ex)
            {

                Foundations.Common.Services.StaticLoggingService.LogError(ex);
            }
            Foundations.Common.Services.StaticLoggingService.Log($"EventListenerConsumer [Finished]]");
        }
    }
}
