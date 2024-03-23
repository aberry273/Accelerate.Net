using Accelerate.Foundations.EventPipelines.EventBus;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Contracts;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Accelerate.Features.Content.Consumers
{

    public class DataUpdateConsumer<T, B> : IConsumer<UpdateDataContract<T>> where B : IDataBus<T>
    {
        readonly ILogger<DataUpdateConsumer<T, B>> _logger;
        IDataUpdateEventPipeline<T> _pipeline;
        readonly Bind<B, IPublishEndpoint> _publishEndpoint;
        public DataUpdateConsumer(
            IDataUpdateEventPipeline<T> pipeline,
            Bind<B, IPublishEndpoint> publishEndpoint,
            ILogger<DataUpdateConsumer<T, B>> logger)
        {
            _logger = logger;
            _pipeline = pipeline;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<UpdateDataContract<T>> context)
        {
            Foundations.Common.Services.StaticLoggingService.Log($"DataUpdatePipeline [Started]]");
            // Run synchronous pipeline first
            _pipeline.Run(context.Message.Data);
            //Run async pipeline second
            await _pipeline.RunAsync(context.Message.Data);
            // Emit complete event
            await _publishEndpoint.Value.Publish(new UpdateCompleteDataContract<T>() { Data = context.Message.Data });
            Foundations.Common.Services.StaticLoggingService.Log($"DataUpdatePipeline [Finished]]");
        }
    }
}
