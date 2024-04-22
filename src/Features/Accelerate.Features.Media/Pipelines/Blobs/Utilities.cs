using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Media.Pipelines.Blobs
{
    public static class Utilities
    {
        public static async Task SendWebsocketUpdate(IHubContext<BaseHub<MediaBlobDocument>, IBaseHubClient<WebsocketMessage<MediaBlobDocument>>> messageHub, IPipelineArgs<MediaBlobDocument> args, string message, DataRequestCompleteType type)
        {
            var payload = new WebsocketMessage<MediaBlobDocument>()
            {
                Message = message,
                Code = 200,
                Data = args.Value,
                UpdateType = type,
                Group = "Media",
            };
            await messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}
