using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Activities
{
    public static class ContentPostActivityUtilities
    {
        public static ContentPostActionsSummaryDocument GetActivities(IEntityService<ContentPostActivityEntity> entityService, IPipelineArgs<ContentPostActivityEntity> args)
        {
            return entityService
               .Find(x => x.ContentPostId == args.Value.ContentPostId)
               .GroupBy(g => 1)
               .Select(x =>
                   new ContentPostActionsSummaryDocument
                   {
                       Agrees = x
                           .Where(y => y.Type == ContentPostActivityTypes.Agree)
                           .OrderByDescending(y => y.CreatedOn)
                           .GroupBy(y => y.UserId)
                           .First()
                           .Count(y => y.Value == "true"),
                       Disagrees = x
                           .Where(y => y.Type == ContentPostActivityTypes.Disagree)
                           .OrderByDescending(y => y.CreatedOn)
                           .GroupBy(y => y.UserId)
                           .First()
                           .Count(y => y.Value == "true"),
                       Likes = x
                           .Where(y => y.Type == ContentPostActivityTypes.Like)
                           .OrderByDescending(y => y.CreatedOn)
                           .GroupBy(y => y.UserId)
                           .First()
                           .Count(y => y.Value == "true"),
                   }).Single();
        }

        // TODO: Potentially remove or add, unsure of userbased state mgmt yet
        public static async Task SendWebsocketActivityUpdate(IHubContext<BaseHub<ContentPostActivityDocument>, IBaseHubClient<WebsocketMessage<ContentPostActivityDocument>>> messageHub, IPipelineArgs<ContentPostActivityDocument> args, string message, DataRequestCompleteType type)
        {
            var payload = new WebsocketMessage<ContentPostActivityDocument>()
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
