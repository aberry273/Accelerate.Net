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
    public class ContentPostActionsCreatedCompletedListenerPipeline : DataCreateCompletedEventPipeline<ContentPostActionsEntity>
    {
        IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> _messageHub;
        IEntityService<ContentPostActionsEntity> _entityActionsService;
        IEntityService<ContentPostActivityEntity> _entityActivitiesService;
        IElasticService<ContentPostActionsSummaryDocument> _elasticService;
        IEntityService<ContentPostActionsSummaryEntity> _entityService;
        IElasticService<ContentPostDocument> _elasticPostService;
        public ContentPostActionsCreatedCompletedListenerPipeline(
            IEntityService<ContentPostActionsSummaryEntity> entityService, 
            IEntityService<ContentPostActionsEntity> entityActionsService,
            IEntityService<ContentPostActivityEntity> entityActivitiesService,
            IElasticService<ContentPostActionsSummaryDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> messageHub)
        {
            _entityService = entityService;
            _entityActionsService = entityActionsService;
            _entityActivitiesService = entityActivitiesService;
            _elasticPostService = elasticPostService;
            _elasticService = elasticService;
            _messageHub = messageHub;

            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActionsEntity>>()
            {
                UpdateIndex
            };
        }

        public async Task UpdateIndex(IPipelineArgs<ContentPostActionsEntity> args)
        {
            if(!args.Value.Agree.GetValueOrDefault() && !args.Value.Disagree.GetValueOrDefault())
            {
                return;
            }
            var entity = _entityService.Find(x => x.ContentPostId == args.Value.ContentPostId).FirstOrDefault();
            
            var response = await _elasticService.GetDocument<ContentPostActionsSummaryDocument>(entity.Id.ToString());
            var doc = response.Source;
            var newDoc = doc == null;
            if (newDoc)
            {
                doc = new ContentPostActionsSummaryDocument()
                {
                    Id = entity.Id,
                    ContentPostId = args.Value.ContentPostId,
                    UserId = args.Value.UserId,
                };
            }
            
            var actionscounts = ContentPostActionsSummaryUtilities.GetActionCounts(_entityActionsService, args);
            doc.Agrees = actionscounts.Agrees;
            doc.Disagrees = actionscounts.Disagrees;
            doc.Likes = actionscounts.Likes;

            if (newDoc)
            {
                await _elasticService.Index(doc);
                await ContentPostActionsSummaryUtilities.SendWebsocketActionsSummaryUpdate(_messageHub, doc, "Created", DataRequestCompleteType.Created);
            }
            else
            {
                await _elasticService.UpdateDocument(doc, doc.Id.ToString());
                await ContentPostActionsSummaryUtilities.SendWebsocketActionsSummaryUpdate(_messageHub, doc, "Updated", DataRequestCompleteType.Updated);
            }
            //do stuff
            var postUpdate = new ContentPostDocument()
            {
                ActionsTotals = doc
            };
            var result = await _elasticPostService.UpdateDocument(postUpdate, args.Value.ContentPostId.ToString());


        }
    }
}
