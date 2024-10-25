using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Actions
{
    public static class ContentChannelsUtilities
    { 

        // TODO: Potentially remove or add, unsure of userbased state mgmt yet
        public static async Task SendWebsocketUpdate(IHubContext<BaseHub<ContentChannelDocument>, IBaseHubClient<WebsocketMessage<ContentChannelDocument>>> messageHub, IPipelineArgs<ContentChannelDocument> args, string message, DataRequestCompleteType type)
        {
            var payload = new WebsocketMessage<ContentChannelDocument>()
            {
                Message = message,
                Code = 200,
                Data = args.Value,
                UpdateType = type,
                Group = "Action",
            };
            await messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}
