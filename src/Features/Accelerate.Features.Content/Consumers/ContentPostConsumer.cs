using Accelerate.Features.Content.Models.Contracts;
using Accelerate.Features.Content.Pipelines;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Integrations.Contracts;
using MassTransit;

namespace Accelerate.Features.Content.Consumers
{

    public class ContentPostConsumer : IConsumer<ContentPostContract>
    {
        readonly ILogger<ContentPostConsumer> _logger;
        IContentPostCreatePipeline _pipeline;

        public ContentPostConsumer(IContentPostCreatePipeline pipeline, ILogger<ContentPostConsumer> logger)
        {
            _logger = logger;
            _pipeline = pipeline;
        }

        public Task Consume(ConsumeContext<ContentPostContract> context)
        {
            //Run pipeline
            //_logger.LogInformation("Received Text: {Text}", context.Message.Value);
            _pipeline.RunAsync(context.Message.Entity);
            return Task.CompletedTask;
        }
    }
}
