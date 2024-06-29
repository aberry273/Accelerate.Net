using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Actions
{
    public static class ContentPostActionUtilities
    {
        public static ContentPostActionsSummaryDocument GetActions(IEntityService<ContentPostActionsEntity> entityService, IPipelineArgs<ContentPostActionsEntity> args)
        {
            return entityService
               .Find(x => x.ContentPostId == args.Value.ContentPostId)
               .GroupBy(g => 1)
               .Select(x =>
                   new ContentPostActionsSummaryDocument
                   {
                       Agrees = x.Count(y => y.Agree == true),
                       Disagrees = x.Count(y => y.Disagree == true),
                       //Likes = x.Count(y => y.Like == true),
                   }).Single();
        }

        // TODO: Potentially remove or add, unsure of userbased state mgmt yet
        public static async Task SendWebsocketActionUpdate(IHubContext<BaseHub<ContentPostActionsDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsDocument>>> messageHub, IPipelineArgs<ContentPostActionsDocument> args, string message, DataRequestCompleteType type)
        {
            var payload = new WebsocketMessage<ContentPostActionsDocument>()
            {
                Message = message,
                Code = 200,
                Data = args.Value,
                UpdateType = type,
                Group = "Action",
            };
            await messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
        public static async Task SendWebsocketPostUpdate(IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts, string userId, ContentPostDocument doc, DataRequestCompleteType type)
        {
            var payload = new WebsocketMessage<ContentPostDocument>()
            {
                Message = "Update successful",
                Code = 200,
                Data = doc,
                UpdateType = type,
                Group = "Post"
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(userId);
            await messageHubPosts.Clients.Clients(userConnections).SendMessage(userId, payload);
        }
    }
}
