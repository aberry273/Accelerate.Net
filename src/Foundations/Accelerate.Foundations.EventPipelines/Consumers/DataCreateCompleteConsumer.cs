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

    public class DataCreateCompleteConsumer<T> : IConsumer<CreateCompleteDataContract<T>>
    {
        readonly ILogger<DataCreateCompleteConsumer<T>> _logger;
        IHubContext<BaseHub<T>, IBaseHubClient<WebsocketMessage<T>>> _messageHub;
        public DataCreateCompleteConsumer(
            IHubContext<BaseHub<T>, IBaseHubClient<WebsocketMessage<T>>> messageHub,
            ILogger<DataCreateCompleteConsumer<T>> logger)
        {
            _logger = logger;
            _messageHub = messageHub;
        }

        public async Task Consume(ConsumeContext<CreateCompleteDataContract<T>> context)
        {
            // RUn end comple
            Foundations.Common.Services.StaticLoggingService.Log($"DataCreateCompleteConsumer [Started]]");
            var payload = new WebsocketMessage<T>()
            {
                Message = "Create successful",
                Code = 200,
                Data = context.Message.Data,
                UpdateType = DataRequestCompleteType.Created,
            };
            //await _messageHub.Clients.Group(context.Message.Target).SendMessage(context.Message.UserId.ToString(), payload);
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(context.Message.UserId.ToString());
            await _messageHub.Clients.Clients(userConnections).SendMessage(context.Message.UserId.ToString(), payload);
            //await _messageHub.Clients.All.SendMessage("user", payload);
            Foundations.Common.Services.StaticLoggingService.Log($"DataCreateCompleteConsumer [Complete]]");
        }
    }
}
