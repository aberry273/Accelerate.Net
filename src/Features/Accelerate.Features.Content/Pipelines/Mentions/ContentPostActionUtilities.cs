using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Mentions
{
    public static class ContentPostMentionUtilities
    { 

        // TODO: Potentially remove or add, unsure of userbased state mgmt yet
        public static async Task SendWebsocketMentionUpdate(IHubContext<BaseHub<ContentPostMentionDocument>, IBaseHubClient<WebsocketMessage<ContentPostMentionDocument>>> messageHub, IPipelineArgs<ContentPostMentionDocument> args, string message, DataRequestCompleteType type)
        {
            var payload = new WebsocketMessage<ContentPostMentionDocument>()
            {
                Message = message,
                Code = 200,
                Data = args.Value,
                UpdateType = type,
                Group = "Mention",
            };
            await messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}
