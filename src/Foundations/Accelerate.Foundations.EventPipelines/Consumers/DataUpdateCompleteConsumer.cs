using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Websockets.Hubs;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Accelerate.Features.Content.Consumers
{

    public class DataUpdateCompleteConsumer<T> : IConsumer<UpdateCompleteDataContract<T>>
    {
        readonly ILogger<DataUpdateCompleteConsumer<T>> _logger;
        IHubContext<BaseHub<T>, IBaseHubClient<T>> _messageHub;
        public DataUpdateCompleteConsumer(
            IHubContext<BaseHub<T>, IBaseHubClient<T>> messageHub,
            ILogger<DataUpdateCompleteConsumer<T>> logger)
        {
            _logger = logger;
            _messageHub = messageHub;
        }

        public async Task Consume(ConsumeContext<UpdateCompleteDataContract<T>> context)
        {
            // RUn end comple
            Foundations.Common.Services.StaticLoggingService.Log($"DataUpdateCompleteConsumer [Started]]");
            await _messageHub.Clients.All.SendMessage("user", context.Message.Data);
            Foundations.Common.Services.StaticLoggingService.Log($"DataUpdateCompleteConsumer [Complete]]");
        }
    }
}
