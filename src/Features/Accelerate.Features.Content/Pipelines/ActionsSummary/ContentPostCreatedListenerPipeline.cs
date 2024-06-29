using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MSearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Accelerate.Foundations.Content.Hydrators;
using System;

namespace Accelerate.Features.Content.Pipelines.ActionsSummary
{
    public class ContentPostCreatedListenerPipeline : DataCreateEventPipeline<ContentPostParentEntity>
    {
        IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> _messageHub;
        IEntityService<ContentPostActionsEntity> _entityActionsService;
        IEntityService<ContentPostActivityEntity> _entityActivitiesService;
        IElasticService<ContentPostActionsSummaryDocument> _elasticService;
        IEntityService<ContentPostActionsSummaryEntity> _entityService;
        public ContentPostCreatedListenerPipeline(
            IEntityService<ContentPostActionsSummaryEntity> entityService, 
            IEntityService<ContentPostActionsEntity> entityActionsService,
            IEntityService<ContentPostActivityEntity> entityActivitiesService,
            IElasticService<ContentPostActionsSummaryDocument> elasticService,
            IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> messageHub)
        {
            _entityService = entityService;
            _entityActionsService = entityActionsService;
            _entityActivitiesService = entityActivitiesService;
            _elasticService = elasticService;
            _messageHub = messageHub;

            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostParentEntity>>()
            {
                UpdateIndex
            };
        }
        
        public async Task<ContentPostActionsSummaryEntity> GetOrCreateEntity(IPipelineArgs<ContentPostParentEntity> args)
        {
            var entity = _entityService.Find(x => x.ContentPostId == args.Value.ParentId).FirstOrDefault();
            if(entity == null)
            {
                entity = new ContentPostActionsSummaryEntity()
                {
                    ContentPostId = args.Value.Id,
                    //Agrees = CountActivity(args.Value, ContentPostActivityTypes.Agree),
                    //Disagrees = CountActivity(args.Value, ContentPostActivityTypes.Disagree),
                };
                var guid = await _entityService.CreateWithGuid(entity);
                return _entityService.Get(guid.GetValueOrDefault());
            }
            // we skip updating to save on unnessary sql updates
            // todo: update async jobs to perform overall updates to post counters
            return entity;
        }

        public async Task UpdateIndex(IPipelineArgs<ContentPostParentEntity> args)
        {
            var entity = await GetOrCreateEntity(args);
            
            var activitycounts = ContentPostActionsSummaryUtilities.GetActivityCounts(_entityActivitiesService, args);
            var response = await _elasticService.GetDocument<ContentPostActionsSummaryDocument>(entity.Id.ToString());
            var doc = response.Source;
            if (doc == null)
            {
                doc = new ContentPostActionsSummaryDocument()
                {
                    Id = entity.Id,
                    ContentPostId = args.Value.Id,
                    UserId = args.Value.UserId,
                    Quotes = activitycounts.Quotes,
                    Replies = activitycounts.Replies + 1,
            };
                await _elasticService.Index(doc);
                await ContentPostActionsSummaryUtilities.SendWebsocketActionsSummaryUpdate(_messageHub, doc, "Created", DataRequestCompleteType.Created);
            }
            else
            {
                doc.Quotes = activitycounts.Quotes;
                doc.Replies = activitycounts.Replies + 1;
                await _elasticService.UpdateDocument(doc, doc.Id.ToString());
                await ContentPostActionsSummaryUtilities.SendWebsocketActionsSummaryUpdate(_messageHub, doc, "Updated", DataRequestCompleteType.Updated);
            }
            //do stuff
        }
    }
}
