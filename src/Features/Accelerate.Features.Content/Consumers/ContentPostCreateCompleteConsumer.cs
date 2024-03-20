using Accelerate.Features.Content.Models.Contracts;
using Accelerate.Features.Content.Pipelines;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Integrations.Contracts;
using MassTransit;
using MassTransit.DependencyInjection;

namespace Accelerate.Features.Content.Consumers
{

    public class ContentPostCreateCompleteConsumer : IConsumer<ContentPostCreateCompleteContract>
    {
        readonly ILogger<ContentPostCreateCompleteContract> _logger;
        IContentPostCreatedPipeline _pipeline;
        readonly Bind<IContentBus, IPublishEndpoint> _publishEndpoint;
        public ContentPostCreateCompleteConsumer(
            IContentPostCreatedPipeline pipeline,
            Bind<IContentBus, IPublishEndpoint> publishEndpoint,
            ILogger<ContentPostCreateCompleteContract> logger)
        {
            _logger = logger;
            _pipeline = pipeline;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<ContentPostCreateCompleteContract> context)
        {
            // RUn end comple
            Foundations.Common.Services.StaticLoggingService.Log($"ContentPostCreatePipeline: {context.Message.Entity.Id} [Complete]]");
        }
    }
}
