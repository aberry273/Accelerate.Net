using Accelerate.Foundations.EventPipelines.Models;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Accelerate.Features.Content.Consumers
{

    public class DataUpdateCompleteConsumer<T> : IConsumer<UpdateCompleteDataContract<T>>
    {
        readonly ILogger<DataUpdateCompleteConsumer<T>> _logger;
        IHubContext<BaseHub<T>, IBaseHubClient<WebsocketMessage<T>>> _messageHub;
        public DataUpdateCompleteConsumer(
            IHubContext<BaseHub<T>, IBaseHubClient<WebsocketMessage<T>>> messageHub,
            ILogger<DataUpdateCompleteConsumer<T>> logger)
        {
            _logger = logger;
            _messageHub = messageHub;
        }

        public async Task Consume(ConsumeContext<UpdateCompleteDataContract<T>> context)
        {
            // RUn end comple
            Foundations.Common.Services.StaticLoggingService.Log($"DataUpdateCompleteConsumer [Started]]");
            var payload = new WebsocketMessage<T>()
            {
                Message = "Update successful",
                Code = 200,
                Data = context.Message.Data,
                UpdateType = DataRequestCompleteType.Updated
            };
            //await _messageHub.Clients.Group(context.Message.Target).SendMessage(context.Message.UserId.ToString(), payload);
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(context.Message.UserId.ToString());
            await _messageHub.Clients.Clients(userConnections).SendMessage(context.Message.UserId.ToString(), payload);
            Foundations.Common.Services.StaticLoggingService.Log($"DataUpdateCompleteConsumer [Complete]]");
        }
    }
}
