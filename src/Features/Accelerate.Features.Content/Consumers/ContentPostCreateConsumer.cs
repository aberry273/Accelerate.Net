using Accelerate.Features.Content.Models.Contracts;
using Accelerate.Features.Content.Pipelines;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Integrations.Contracts;
using MassTransit;
using MassTransit.DependencyInjection;

namespace Accelerate.Features.Content.Consumers
{

    public class ContentPostCreateConsumer : IConsumer<ContentPostCreateContract>
    {
        readonly ILogger<ContentPostCreateConsumer> _logger;
        IContentPostCreatedPipeline _pipeline;
        readonly Bind<IContentBus, IPublishEndpoint> _publishEndpoint;
        public ContentPostCreateConsumer(
            IContentPostCreatedPipeline pipeline,
            Bind<IContentBus, IPublishEndpoint> publishEndpoint,
            ILogger<ContentPostCreateConsumer> logger)
        {
            _logger = logger;
            _pipeline = pipeline;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<ContentPostCreateContract> context)
        {
            Foundations.Common.Services.StaticLoggingService.Log($"ContentPostCreatePipeline: {context.Message.Entity.Id} [Started]]");
            // Run synchronous pipeline first
            _pipeline.Run(context.Message.Entity);
            //Run async pipeline second
            await _pipeline.RunAsync(context.Message.Entity);
            // Emit complete event
            await _publishEndpoint.Value.Publish(new ContentPostCreateCompleteContract(context.Message.Entity));
        }
    }
}
