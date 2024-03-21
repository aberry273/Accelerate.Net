using Accelerate.Foundations.EventPipelines.EventBus;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Contracts;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Accelerate.Features.Content.Consumers
{

    public class DataCreateConsumer<T, B> : IConsumer<CreateDataContract<T>> where B : IDataBus<T>
    {
        readonly ILogger<DataCreateConsumer<T, B>> _logger;
        IDataEventPipeline<T> _pipeline;
        readonly Bind<B, IPublishEndpoint> _publishEndpoint;
        public DataCreateConsumer(
            IDataEventPipeline<T> pipeline,
            Bind<B, IPublishEndpoint> publishEndpoint,
            ILogger<DataCreateConsumer<T, B>> logger)
        {
            _logger = logger;
            _pipeline = pipeline;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<CreateDataContract<T>> context)
        {
            Foundations.Common.Services.StaticLoggingService.Log($"DataCreatePipeline [Started]]");
            // Run synchronous pipeline first
            _pipeline.Run(context.Message.Data);
            //Run async pipeline second
            await _pipeline.RunAsync(context.Message.Data);
            // Emit complete event
            await _publishEndpoint.Value.Publish(new CreateCompleteDataContract<T>() { Data = context.Message.Data });
            Foundations.Common.Services.StaticLoggingService.Log($"DataCreatePipeline [Finished]]");
        }
    }
}
