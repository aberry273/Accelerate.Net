using Accelerate.Foundations.EventPipelines.EventBus;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Contracts;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Accelerate.Features.Content.Consumers
{

    public class DataDeleteConsumer<T, B> : IConsumer<DeleteDataContract<T>> where B : IDataBus<T>
    {
        readonly ILogger<DataDeleteConsumer<T, B>> _logger;
        IDataDeleteEventPipeline<T> _pipeline;
        readonly Bind<B, IPublishEndpoint> _publishEndpoint;
        public DataDeleteConsumer(
            IDataDeleteEventPipeline<T> pipeline,
            Bind<B, IPublishEndpoint> publishEndpoint,
            ILogger<DataDeleteConsumer<T, B>> logger)
        {
            _logger = logger;
            _pipeline = pipeline;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<DeleteDataContract<T>> context)
        {
            Foundations.Common.Services.StaticLoggingService.Log($"DataDeletePipeline [Started]]");
            // Run synchronous pipeline first
            _pipeline.Run(context.Message.Data);
            //Run async pipeline second
            await _pipeline.RunAsync(context.Message.Data);
            // Emit complete event
            await _publishEndpoint.Value.Publish(new DeleteCompleteDataContract<T>() { Data = context.Message.Data });
            Foundations.Common.Services.StaticLoggingService.Log($"DataDeletePipeline [Finished]]");
        }
    }
}
