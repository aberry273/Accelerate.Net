using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Websockets.Hubs;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Accelerate.Features.Content.Consumers
{

    public class DataDeleteCompleteConsumer<T> : IConsumer<DeleteCompleteDataContract<T>>
    {
        readonly ILogger<DataDeleteCompleteConsumer<T>> _logger;
        IHubContext<BaseHub<T>, IBaseHubClient<T>> _messageHub;
        public DataDeleteCompleteConsumer(
            IHubContext<BaseHub<T>, IBaseHubClient<T>> messageHub,
            ILogger<DataDeleteCompleteConsumer<T>> logger)
        {
            _logger = logger;
            _messageHub = messageHub;
        }

        public async Task Consume(ConsumeContext<DeleteCompleteDataContract<T>> context)
        {
            // RUn end comple
            Foundations.Common.Services.StaticLoggingService.Log($"DataDeleteCompleteConsumer [Started]]");
            await _messageHub.Clients.All.SendMessage("user", context.Message.Data);
            Foundations.Common.Services.StaticLoggingService.Log($"DataDeleteCompleteConsumer [Complete]]");
        }
    }
}
