using Accelerate.Features.Content.Models.Contracts;
using Accelerate.Features.Content.Pipelines;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Integrations.Contracts;
using Accelerate.Foundations.Websockets.Hubs;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Consumers
{

    public class ContentPostCreateCompleteConsumer : IConsumer<ContentPostCreateCompleteContract>
    {
        readonly ILogger<ContentPostCreateCompleteContract> _logger;
        IHubContext<BaseHub<ContentPostEntity>, IBaseHubClient<ContentPostEntity>> _messageHub;
        public ContentPostCreateCompleteConsumer(
            IHubContext<BaseHub<ContentPostEntity>, IBaseHubClient<ContentPostEntity>> messageHub,
            ILogger<ContentPostCreateCompleteContract> logger)
        {
            _logger = logger;
            _messageHub = messageHub;
        }

        public async Task Consume(ConsumeContext<ContentPostCreateCompleteContract> context)
        {
            // RUn end comple
            var entity = context.Message.Entity;
            await _messageHub.Clients.All.SendMessage("user", entity);
            Foundations.Common.Services.StaticLoggingService.Log($"ContentPostCreatePipeline: {context.Message.Entity.Id} [Complete]]");
        }
    }
}
