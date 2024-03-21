using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Websockets.Hubs;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Accelerate.Features.Content.Consumers
{

    public class DataCreateCompleteConsumer<T> : IConsumer<CreateCompleteDataContract<T>>
    {
        readonly ILogger<DataCreateCompleteConsumer<T>> _logger;
        IHubContext<BaseHub<T>, IBaseHubClient<T>> _messageHub;
        public DataCreateCompleteConsumer(
            IHubContext<BaseHub<T>, IBaseHubClient<T>> messageHub,
            ILogger<DataCreateCompleteConsumer<T>> logger)
        {
            _logger = logger;
            _messageHub = messageHub;
        }

        public async Task Consume(ConsumeContext<CreateCompleteDataContract<T>> context)
        {
            // RUn end comple
            Foundations.Common.Services.StaticLoggingService.Log($"DataCreateCompleteConsumer [Started]]");
            await _messageHub.Clients.All.SendMessage("user", context.Message.Data);
            Foundations.Common.Services.StaticLoggingService.Log($"DataCreateCompleteConsumer [Complete]]");
        }
    }
}
