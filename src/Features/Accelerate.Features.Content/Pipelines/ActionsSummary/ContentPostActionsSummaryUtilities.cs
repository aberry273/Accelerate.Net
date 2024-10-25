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

        public static ContentPostActionsSummaryEntity GetActionSummaryEntity(IEntityService<ContentPostActionsSummaryEntity> entityService, IPipelineArgs<ContentPostActivityEntity> args)
        {
            return entityService
               .Find(x => x.ContentPostId == args.Value.SourceId)
               .FirstOrDefault();
        }
        public static async Task<int> CreateActionSummary(IEntityService<ContentPostActionsSummaryEntity> entityService, ContentPostActionsSummaryEntity entity)
        {
            return await entityService
                .Create(entity);
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
                       Reactions = x.Count(y => !string.IsNullOrEmpty(y.Reaction)),
                   }).Single();
        }
        public static async Task CreateOrUpdateActionsSummaryIndex(IElasticService<ContentPostActionsSummaryDocument> _elasticService, ContentPostActionsSummaryEntity actionsSummary)
        {
            var fetchResponse = await _elasticService.GetDocument<ContentPostActionsSummaryDocument>(actionsSummary.Id.ToString());
            var contentPostDocument = fetchResponse.Source;
            if(contentPostDocument != null)
            {
                contentPostDocument.Agrees = actionsSummary.Agrees;
                contentPostDocument.Disagrees = actionsSummary.Disagrees;
                contentPostDocument.Replies = actionsSummary.Replies;
                contentPostDocument.Quotes = actionsSummary.Quotes;

                await _elasticService.UpdateOrCreateDocument(contentPostDocument, actionsSummary.Id.ToString());
            }
            else
            {
                contentPostDocument = new ContentPostActionsSummaryDocument()
                {
                    Agrees = actionsSummary.Agrees,
                    Disagrees = actionsSummary.Disagrees,
                    ContentPostId = actionsSummary.ContentPostId,
                    Id = actionsSummary.Id,
                    Quotes = actionsSummary.Quotes,
                    Replies = actionsSummary.Replies,
                };
                await _elasticService.Index(contentPostDocument);
            }
        }

        /// <summary>
        ///  OLD
        /// </summary>
        /// <param name="entityService"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ContentPostActionsSummaryDocument GetActionsCount(IEntityService<ContentPostActionsEntity> entityService, IPipelineArgs<ContentPostActionsEntity> args)
        {
            return entityService
               .Find(x => x.ContentPostId == args.Value.ContentPostId)
               .GroupBy(g => 1)
               .Select(x =>
                   new ContentPostActionsSummaryDocument
                   {
                       Agrees = x.Count(y => y.Agree == true),
                       Disagrees = x.Count(y => y.Disagree == true),
                       Likes = x.Count(y => y.Like == true),
                   }).Single();
        }
        public static ContentPostActionsSummaryDocument GetActivityCounts(IEntityService<ContentPostActivityEntity> entityService, IPipelineArgs<ContentPostActivityEntity> args)
        { 
            var a = entityService
               .Find(x => x.SourceId == args.Value.SourceId)?
               .GroupBy(g => 1)?
               .Select(x => x);

            return entityService
               .Find(x => x.SourceId == args.Value.SourceId)?
               .GroupBy(g => 1)?
               .Select(x =>
                   new ContentPostActionsSummaryDocument
                   {
                       Replies = x
                           .Where(y => y.Type == ContentPostActivityTypes.Reply)?
                           .GroupBy(y => y.UserId)?
                           .FirstOrDefault()?
                           .Count() ?? 0,
                       Quotes = x
                           .Where(y => y.Type == ContentPostActivityTypes.Quote)?
                           .GroupBy(y => y.UserId)?
                           .FirstOrDefault()?
                           .Count() ?? 0,
                   })?.FirstOrDefault()
                   ?? new ContentPostActionsSummaryDocument();
        }

        // TODO: Potentially remove or add, unsure of userbased state mgmt yet
        public static async Task SendWebsocketActionsSummaryUpdate(IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> messageHub, ContentPostActionsSummaryDocument doc, string message, DataRequestCompleteType type)
        {
            var payload = new WebsocketMessage<ContentPostActionsSummaryDocument>()
            {
                Message = message,
                Code = 200,
                Data = doc,
                UpdateType = type,
                Group = "ActionSummary",
            };
            await messageHub.Clients.All.SendMessage(doc.UserId.ToString(), payload);
        } 
    }
}
