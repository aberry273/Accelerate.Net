using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Reviews
{
    public static class ContentPostUtilities
    {
        public static ContentPostReviewsDocument? GetReplies(IEntityService<ContentPostEntity> entityService, IPipelineArgs<ContentPostEntity> args)
        {
            return entityService
               .Find(x => x.ParentId == args.Value.ParentId && x.Type != ContentPostType.Page)
               .GroupBy(g => 1)
               .Select(x =>
                   new ContentPostReviewsDocument
                   {
                       Replies = x.Count(),
                   })?.SingleOrDefault();
        } 
        public static async Task SendWebsocketPostUpdate(IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts, string userId, ContentPostDocument doc, DataRequestCompleteType type)
        {
            var payload = new WebsocketMessage<ContentPostDocument>()
            {
                Message = "Update successful",
                Code = 200,
                Data = doc,
                UpdateType = type,
                Group = "Post",
                Alert = false
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(userId);
            await messageHubPosts.Clients.Clients(userConnections).SendMessage(userId, payload);
        }
        public static async Task IndexOwnReplies(IElasticService<ContentPostDocument> elasticService, IEntityService<ContentPostEntity> entityService, IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts, IPipelineArgs<ContentPostEntity> args)
        {
            // Get total replies
            var parentResponse = await elasticService.GetDocument<ContentPostDocument>(args.Value.ParentId.ToString());
            if (!parentResponse.IsValidResponse) return;
            var parentDoc = parentResponse.Source;
            var reviewsDoc = ContentPostUtilities.GetReplies(entityService, args);
            parentDoc.Replies = reviewsDoc?.Replies ?? 0;
            await elasticService.UpdateDocument<ContentPostDocument>(parentDoc, parentDoc.Id.ToString());
            //SendWebsocketPostUpdate(messageHubPosts, parentDoc.UserId.ToString(), parentDoc, DataRequestCompleteType.Updated);
        } 
    }
}
