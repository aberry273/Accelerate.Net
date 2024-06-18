using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.ActionsSummary
{
    public static class ContentPostActionsSummaryUtilities
    {

        public static ContentPostActionsSummaryEntity GetActionSummaryEntity(IEntityService<ContentPostActionsSummaryEntity> entityService, IPipelineArgs<ContentPostActionsEntity> args)
        {
            return entityService
               .Find(x => x.ContentPostId == args.Value.ContentPostId)
               .FirstOrDefault();
        }
        public static async Task<int> CreateActionSummary(IEntityService<ContentPostActionsSummaryEntity> entityService, ContentPostActionsSummaryEntity entity)
        {
            return await entityService
                .Create(entity);
        }
        public static ContentPostActionsSummaryEntity CreateActionSummaryEntity(IPipelineArgs<ContentPostActionsEntity> args)
        {
            return new ContentPostActionsSummaryEntity()
            {
                ContentPostId = args.Value.ContentPostId,
                Agrees = args.Value.Agree.GetValueOrDefault() ? 1 : 0,
                Disagrees = args.Value.Disagree.GetValueOrDefault() ? 1 : 0,
            };
        }

        public static ContentPostActionsSummaryDocument GetActionCounts(IEntityService<ContentPostActionsEntity> entityService, IPipelineArgs<ContentPostActionsEntity> args)
        {
            return entityService
               .Find(x => x.ContentPostId == args.Value.ContentPostId)
               .GroupBy(g => 1)
               .Select(x =>
                   new ContentPostActionsSummaryDocument
                   {
                       Agrees = x.Count(y => y.Agree == true),
                       Disagrees = x.Count(y => y.Disagree == true),
                   }).Single();
        }
        public static async Task CreateOrUpdateActionsSummaryIndex(IElasticService<ContentPostActionsSummaryDocument> _elasticService, ContentPostActionsEntity actionsSummary)
        {

            var fetchResponse = await _elasticService.GetDocument<ContentPostActionsSummaryDocument>(actionsSummary.Id.ToString());
            var contentPostDocument = fetchResponse.Source;
            //await _elasticPostService.UpdateOrCreateDocument(contentPostDocument, args.Value?.ContentPostId.ToString())
        }

        /// <summary>
        ///  OLD
        /// </summary>
        /// <param name="entityService"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        

        public static ContentPostActionsSummaryDocument GetActivities(IEntityService<ContentPostActivityEntity> entityService, IPipelineArgs<ContentPostActionsSummaryEntity> args)
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
        public static async Task SendWebsocketActionsSummaryUpdate(IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> messageHub, IPipelineArgs<ContentPostActionsSummaryDocument> args, string message, DataRequestCompleteType type)
        {
            var payload = new WebsocketMessage<ContentPostActionsSummaryDocument>()
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
