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
    public class ContentPostActionsUpdatedListenerPipeline : DataUpdateEventPipeline<ContentPostActivityEntity>
    {
        IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> _messageHub;
        IEntityService<ContentPostActionsEntity> _entityActionsService;
        IEntityService<ContentPostActionsSummaryEntity> _entityService;
        IEntityService<ContentPostActivityEntity> _entityActivitiesService;
        IElasticService<ContentPostActionsSummaryDocument> _elasticService;
        public ContentPostActionsUpdatedListenerPipeline(
            IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> messageHub,
            IEntityService<ContentPostActionsSummaryEntity> entityService,
            IEntityService<ContentPostActivityEntity> entityActivitiesService,
            IEntityService<ContentPostActionsEntity> entityActionsService,
            IElasticService<ContentPostActionsSummaryDocument> elasticService)
        {
            _entityService = entityService;
            _entityActionsService = entityActionsService;
            _messageHub = messageHub;
            _entityActivitiesService = entityActivitiesService;
            _elasticService = elasticService;
            // run summary update of action event

            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActivityEntity>>()
            {
                UpdateEntity,
            };
        }
        public async Task UpdateEntity(IPipelineArgs<ContentPostActivityEntity> args)
        {
            var counts = ContentPostActionsSummaryUtilities.GetActivityCounts(_entityActivitiesService, args);
            var contentPostDoc = new ContentPostDocument()
            {
                ActionsTotals = new ContentPostActionsSummaryDocument()
                {
                    Agrees = counts.Agrees,
                    Disagrees = counts.Disagrees,
                    Quotes = counts.Quotes,
                    Replies = counts.Replies,
                }
            };
            await _elasticService.UpdateDocument(contentPostDoc, args.Value.ContentPostId.ToString());
            //do stuff
        }
        
    }
}
